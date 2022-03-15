# Animacion

## PRACTICA 1
**Curvas Bezier Bicúbicas + Interpolación**:

### El programa

He implementado las curvas y el movimiento de un objeto a través de ellas con un movimiento uniforme que no depende de la curvatura de la curva (segunda derivada = velocidad de la curva, pero no es la que queremos porque se movería más rápido a menor curvatura y más lento a mayor curvatura).

Para ello he utilizado LookUpTables con la distancia acumulada de la curva y su t correspondiente, y las actualizo cada vez que mis puntos de control cambian de posición:

    // PointCloud Update()
    if (transform.hasChanged && bezier)
    {
        bezier.UpdateControlPoints();
        bezier.UpdateLineRenderer();
        transform.hasChanged = false;
    }

Los puntos de control se obtienen como la posición de cada uno de los objetos hijo de la curva:

    // Bezier
    public void UpdateControlPoints()
	{
        ControlPoint[] controlPoints = GetComponentsInChildren<ControlPoint>();
        cpPositions = new Vector3[controlPoints.Length];
        for (int i = 0; i < controlPoints.Length; i++)
        {
            cpPositions[i] = controlPoints[i].transform.position;
        }
		UpdateBezierPoints();
    }

Y actualizo las LookUpTables vaciándolas e iterando por la curva con incrementos de T tan pequeños como precisa quiero que sea. La distancia acumulada será la distancia del t anterior + la distancia entre el punto anterior y el nuevo punto en t.

En mi caso he utilizado una resolución de 0.01.

Además, también actualizo una LookUpTable de puntos de la curva a partir de su t, lo cual se calcularía con la ecuación de la bezier bicúbica, aunque no es tan necesario, solo reduce el tiempo de cálculo. Seguramente sea mejor opción descartar esta LookUpTable ya que mis consultas de puntos de la curva se hacen con valores de t interpolados, por lo que muy difílmente caerá precisamente en un punto ya calculado. Antes cuando no interpolaba si era útil.

    private void UpdateBezierPoints()
	{
		// Vaciamos las LUT
		LUTpuntosT.Clear();
		LUTdistanceByT.Clear();
		LUTtByDistance.Clear();

		// El parametro de la linea t va incrementado segun la resolucion de la curva
		decimal t = 0;

		// Inicio
		LUTdistanceByT[0] = 0;
		LUTtByDistance[0] = 0;
		LUTpuntosT[0] = cpPositions[0];

		while (t <= 1)
		{
			decimal tAnterior = t;
			t += BezierResolution; // + 0.01

			Vector3 point = GetBezierPointT(t);
			
			LUTpuntosT.Add(t, point);

			// distancia(t1) = ( Distancia entre p(t0) y p(t1) ) + distancia(t0)
			float distancia = (point - LUTpuntosT[tAnterior]).magnitude + LUTdistanceByT[tAnterior];
			LUTdistanceByT.Add(t, distancia);
			if (!LUTtByDistance.ContainsKey(distancia))
				LUTtByDistance.Add(distancia, t);
		}
	}

He implementado como ecuación de Bezier la general y no la bicúbica pensando en si en un futuro podría escalarlo a curvas de Bezier de mayor o menor grado.

    private void UpdateBezierPoints()
	{
		// Vaciamos las LUT
		LUTpuntosT.Clear();
		LUTdistanceByT.Clear();
		LUTtByDistance.Clear();

		// El parametro de la linea t va incrementado segun la resolucion de la curva
		decimal t = 0;

		// Inicio
		LUTdistanceByT[0] = 0;
		LUTtByDistance[0] = 0;
		LUTpuntosT[0] = cpPositions[0];

		while (t <= 1)
		{
			decimal tAnterior = t;
			t += BezierResolution;

			Vector3 point = GetBezierPointT(t);
			
			LUTpuntosT.Add(t, point);

			// distancia(t1) = ( Distancia entre p(t0) y p(t1) ) + distancia(t0)
			float distancia = (point - LUTpuntosT[tAnterior]).magnitude + LUTdistanceByT[tAnterior];
			LUTdistanceByT.Add(t, distancia);
			if (!LUTtByDistance.ContainsKey(distancia))
				LUTtByDistance.Add(distancia, t);
		}
	}

    // COMBINATORIA (usa memoization)
	// Cache con una lista de resultados porque se van a repetir mucho
	private readonly Dictionary<Tuple<float, float>, float> _combCache = new Dictionary<Tuple<float, float>, float>();

Además he implementado memoización para la Combinatoria ya que las llamadas a esta función eran muy frecuentes y las combinatorias que se calculan son poquísimas

	private float Combinatoria(float n, float k)
	{
		Tuple<float, float> key = new Tuple<float, float>(n, k);
		if (_combCache.ContainsKey(key))
			return _combCache[key];

		if (k.Equals(0))
			return 1;

		if (k.Equals(1))
			return n;

		// Combinatoria = n(n-1)(n-2)...(n - k+1) / k!
		float comb = n;
		for (int i = 1; i < k; ++i)
			comb *= n - i;

		// Factorial de k = 1*2*3*4*...*k
		float factorial = 1;
		for (int i = 1; i <= k; ++i)
			factorial *= i;

		_combCache.Add(key, comb / factorial);
		return _combCache[key];
	}

Una vez implementadas las LookUpTable necesitamos métodos para consultarlas, y dado que los valores no son continuos habrá que calcular valores intermedios por medio de Interpolación:

    public float GetDist(decimal t)
	{
		if (t <= 0)
			return 0;

		if (t >= 1)
			return GetLenght();
		
		if (LUTdistanceByT.ContainsKey(t))
			return LUTdistanceByT[t];

		// Si no esta registrado en la LUT buscamos el t mas cercano
		for (decimal t0 = 0; t0 <= 1; t0 += BezierResolution)
		{
			if (t >= t0)
			{
				decimal t1 = t0 + BezierResolution;
				// Remapeado del rango [t0,t1] a [d0,d1] por Interpolacion Lineal
				return ((float)t).Remap((float)t0, (float)t1, LUTdistanceByT[t0], LUTdistanceByT[t1]);
			}
		}

		// Si no interpolamos con distancias continuas a las malas
		return (float)t * GetLenght();
	}

    public decimal GetT(float distance)
	{
		// Casos Triviales:
		// Si se sale de la curva extrapolar
		if (distance <= 0 || distance >= GetLenght())
		{
			return (decimal)(distance / GetLenght());
		}

		// Last distance
		float s0 = 0;
		// Buscar por toda la tabla de distancias hasta que encaje en un hueco y interpolar la t del segmento
		foreach (float s1 in LUTtByDistance.Keys)
		{
			if (distance <= s1 && distance >= s0)
			{
				decimal t0 = LUTtByDistance[s0];
				decimal t1 = LUTtByDistance[s1];
				
				// INTERPOLACION entre los dos puntos t (% deltaS => % deltaT)
				// t = (fraccion de distancia que sobrepasa) * (segmento t) + t0
				return ((decimal)distance).Remap((decimal)s0, (decimal)s1, t0, t1);
			}
			s0 = s1;
		}

		// Interpolamos si no encuentra en la tabla un hueco
		return (decimal)Mathf.InverseLerp(0, GetLenght(), distance);
	}

La interpolación la realizo con un método Remap(a, b, t0, t1) que realiza una interpolación a los valores [a,b] a partir de t, y luego una interpolación inversa al rango [t0, t1] con el resultado, que quedaría así:

    (value - a) / (b - a) * (t1 - t0) + t0

**¿Por qué uso decimal para los valores de t, y float para las distancias en la curva?**

En principio, implementé todo de distinta forma y tuve problemas con la precisión de t al crear la LookUpTable. Cuando incrementas un valor float con un incremento muy pequeño (0.01) tras varias iteraciones perdía precisión y cuando llegaba, por ejemplo, a 0.81, 0.82, 0.83 seguía con 0.839999, 0.849999, 0.859999, por lo que luego al consultar la tabla de valores la t no encajaba la mayoría de las veces, y como no tenía implementada la interpolación aproximaba al más cercano, lo cual daba un resultado muy tosco, como si el objeto fuera a saltos.

Una vez implementada la interpolación ya no era necesario, pero debugear me resultaba más difícil, así que lo dejé así con decimal, el cual tiene la mayor precisión. Ni siquiera el double era capaz de dar un buen resultado si se le incrementaba muchas veces en un incremento pequeño.



### **Ease In / Ease Out**

Fui haciendo iteraciones en mi código conforme iba descifrando las matemáticas de la curva. Al principio tomaba la velocidad y la aceleración de cada Ease como input y calculaba la posición del objeto a partir de estos.

Luego conseguí implementar las ecuaciones de Ease In / Ease Out que obtienen la distancia recorrida a partir de el tiempo de animación y la fracción de curva con cada Ease.

Despejé la velocidad a partir de la última ecuación, aunque estoy seguro que se podría despejar de la primera o la segunda:

    float v0 = 1 / (-t1/2 + 1 - (1-t2)/2);

El tiempo lo normalizo de rango [0, TiempoAnimación] a [0,1]:

    float timeNormalized = Mathf.InverseLerp(0, animationTime, time);

Y para cada fracción de curva, la cual depende del tiempo normalizado, si está antes de t1, después y después de t2, calculo la distancia recorrida a partir de su ecuación:

    // Ease IN
    if (tiempoNormalizado < t1)
        d = v0 * timeNormalized * timeNormalized / 2 / t1;

    // Ease Middle
    if (tiempoNormalizado >= t1 && tiempoNormalizado <= t2)
        d = v0 * t1 / 2 + v0 * (timeNormalized - t1);

    // Ease OUT
    if (tiempoNormalizado > t2)
        d = v0 * t1 / 2 + v0 * (t2 - t1) + (v0 - (v0 * (timeNormalized - t2) / (1 - t2)) / 2) * (timeNormalized - t2);

Desnormalizo la distancia para colocarla sobre la curva y obtener el parámetro t:

    d *= Bezier.GetLenght();

	return (float)Bezier.GetT(d);

Una vez tengo la t de la curva donde el objeto debe estar en un momento de tiempo puedo calcular su posición:

    transform.position = Bezier.GetBezierPointT(t)

Y su rotación, que en realidad gracias a Quaternion.LookRotation(direccion), se puede obtener a partir del vector tangente de la curva, que coincide con la velocidad de la curva que se calcula como la derivada.

Esta rotación se interpola esféricamente con Quaternion.Slerp(Q0, Q1, deltaTime):

    private Quaternion RotateTowardsCurve(float t)
	{
		// Los cuerpos con masa dan problemas con la rotacion
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb)
			rb.mass = 0;

		Vector3 curveVelocity = Bezier.GetVelocity(t).normalized;
		Vector3 up = Vector3.Cross(Vector3.Cross(Bezier.GetAcceleration(t).normalized, curveVelocity), curveVelocity);

		// Apunta en direccion de la Tangente de la Curva (Derivada)
		Quaternion tangentQuat = Quaternion.LookRotation(curveVelocity, up);

		if (rb)
			rb.MoveRotation(
				Quaternion.Slerp(
					transform.rotation,
					tangentQuat,
					(Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime) * Bezier.GetAcceleration(t).magnitude
				)
			);
		else
			transform.rotation = Quaternion.Slerp(
				transform.rotation,
				tangentQuat,
				(Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime) * Bezier.GetAcceleration(t).magnitude
			);

		return tangentQuat;
	}

    
Además, la **aceleración** de la curva, que es la 2ª derivada, nos dice **hacia donde se esta curvando**, como una fuerza que tira de ella, y nos permite calcular un **sistema de coordenadas** sacando la perpendicular con la velocidad, y la perpendicular de ésta y la velocidad, cuyo vector se puede interpretar como el **up** del objeto, lo cual si lo usamos con un avión u objetos similares da resultados mucho más **realistas**. En mi caso, al estar haciendo pruebas con el avión y doblar la curva de forma vertical, la rotación del avión era muy brusca, cuando se va a poner boca abajo pega un giro forzandolo a ponerse boca arriba. Una vez implementado el vector up, el avión se mantiene **boca abajo** de forma realista.

Además, también se puede usar la aceleración como un medidor de la curvatura en ese punto de la curva, por lo que podemos interpolar nuestras rotaciones con una velocidad angular proporcional este valor en QuaternionSlerp(Q0, Q1, deltaTime * P''(t));


### **Control de la Curva**

Para cambiar los puntos de control se pueden mover **arrastrandolos con el ratón**.
Para que fuera cómodo he dejado 2 cámaras estáticas desde arriba y desde el lateral para poder mover los puntos de control en 3D con libertad.

Se puede alternar entre las cámaras con los botones de la esquina.


### Parámetros:

En principio se cambian desde Unity. No he tenido tiempo a implementar una interfaz con sliders ni campos para modificarlos ingame.

Se puede cambiar el recorrido de una curva activando y desactivando Ease In Out Activated, para que sea a velocidad constante si está desactivado (la cámara lo tiene desactivado)

Las fracciones de Ease In y Ease Out se puede modificar entre [0,1] y además no permite superar un máximo de 1 entre la suma de los dos.

También se puede activar o desactivar la rotación y cambiar el tiempo de animación, que es el que tarda en recorrer la curva entera.

Eso está encapsulado en un script llamado "BezierMovable", el cual se debe asignar a cualquier objeto que se quiero dejar recorriendo una curva.

Por otra parte la curva tiene un script "Bezier", donde se puede activar o desactivar la visualización de las lineas y los puntos de control.

Son prefabs y se pueden reutilizar fácilmente independientemente del objeto al que se los asigne.

