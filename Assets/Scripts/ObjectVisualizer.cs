using UnityEngine;

public class ObjectVisualizer : MonoBehaviour
{
    [Header("Visual Settings")]
    public Color lootSpotColor = Color.yellow;
    public Color exitColor = Color.blue;
    public Color noizeColor = Color.red;
    public Color exitRadiusColor = Color.green;
    public Color noizeRadiusColor = Color.red;
    
    private GameObject visualObject;
    private GameObject radiusObject;
    private Light spotLight;
    private Material visualMaterial;
    private float time;
    private ObjectType objectType;
    
    public enum ObjectType
    {
        LootSpot,
        Exit,
        Noize
    }
    
    void Start()
    {
        // Автоматически определяем тип объекта
        DetermineObjectType();
        
        // Создаем визуализацию в зависимости от типа
        CreateVisualization();
    }
    
    void DetermineObjectType()
    {
        if (GetComponent<LootSpot>() != null)
        {
            objectType = ObjectType.LootSpot;
        }
        else if (GetComponent<ExitZone>() != null)
        {
            objectType = ObjectType.Exit;
        }
        else if (GetComponent<NoiseZone>() != null)
        {
            objectType = ObjectType.Noize;
        }
        else
        {
            // Определяем по имени
            string name = gameObject.name.ToLower();
            if (name.Contains("lootspot"))
                objectType = ObjectType.LootSpot;
            else if (name.Contains("exit"))
                objectType = ObjectType.Exit;
            else if (name.Contains("noize"))
                objectType = ObjectType.Noize;
        }
    }
    
    void CreateVisualization()
    {
        switch (objectType)
        {
            case ObjectType.LootSpot:
                CreateLootSpotVisual();
                break;
            case ObjectType.Exit:
                CreateExitVisual();
                break;
            case ObjectType.Noize:
                CreateNoizeVisual();
                break;
        }
    }
    
    void CreateLootSpotVisual()
    {
        // Создаем светящуюся сферу
        visualObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visualObject.transform.SetParent(transform);
        visualObject.transform.localPosition = Vector3.zero;
        visualObject.transform.localScale = Vector3.one * 0.5f;
        
        // Удаляем коллайдер
        Destroy(visualObject.GetComponent<Collider>());
        
        // Создаем материал
        visualMaterial = new Material(Shader.Find("Standard"));
        visualMaterial.color = lootSpotColor;
        visualMaterial.EnableKeyword("_EMISSION");
        visualMaterial.SetColor("_EmissionColor", lootSpotColor);
        visualObject.GetComponent<Renderer>().material = visualMaterial;
        
        // Создаем свет
        GameObject lightObj = new GameObject("SpotLight");
        lightObj.transform.SetParent(transform);
        lightObj.transform.localPosition = Vector3.up * 0.5f;
        
        spotLight = lightObj.AddComponent<Light>();
        spotLight.type = LightType.Point;
        spotLight.color = lootSpotColor;
        spotLight.intensity = 2f;
        spotLight.range = 3f;
    }
    
    void CreateExitVisual()
    {
        // Создаем портал (цилиндр)
        visualObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        visualObject.transform.SetParent(transform);
        visualObject.transform.localPosition = Vector3.zero;
        visualObject.transform.localScale = new Vector3(1f, 0.1f, 1f);
        
        // Удаляем коллайдер
        Destroy(visualObject.GetComponent<Collider>());
        
        // Создаем материал
        visualMaterial = new Material(Shader.Find("Standard"));
        visualMaterial.color = exitColor;
        visualMaterial.EnableKeyword("_EMISSION");
        visualMaterial.SetColor("_EmissionColor", exitColor);
        visualObject.GetComponent<Renderer>().material = visualMaterial;
        
        // Создаем зеленый радиус взаимодействия
        CreateRadius(exitRadiusColor, GetExitRadius());
    }
    
    void CreateNoizeVisual()
    {
        // Создаем куб для шума
        visualObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visualObject.transform.SetParent(transform);
        visualObject.transform.localPosition = Vector3.zero;
        visualObject.transform.localScale = Vector3.one * 0.3f;
        
        // Удаляем коллайдер
        Destroy(visualObject.GetComponent<Collider>());
        
        // Создаем материал
        visualMaterial = new Material(Shader.Find("Standard"));
        visualMaterial.color = noizeColor;
        visualMaterial.EnableKeyword("_EMISSION");
        visualMaterial.SetColor("_EmissionColor", noizeColor);
        visualObject.GetComponent<Renderer>().material = visualMaterial;
        
        // Создаем красный радиус действия
        CreateRadius(noizeRadiusColor, GetNoizeRadius());
    }
    
    void CreateRadius(Color color, float radius)
    {
        if (radius <= 0) return;
        
        radiusObject = new GameObject("Radius");
        radiusObject.transform.SetParent(transform);
        radiusObject.transform.localPosition = Vector3.zero;
        
        LineRenderer lr = radiusObject.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.positionCount = 64;
        lr.useWorldSpace = false;
        lr.loop = true;
        
        // Создаем круг
        float angleStep = 360f / 64f;
        for (int i = 0; i < 64; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            lr.SetPosition(i, new Vector3(x, 0.1f, z));
        }
    }
    
    float GetExitRadius()
    {
        ExitZone exitZone = GetComponent<ExitZone>();
        return exitZone != null ? exitZone.interactRadius : 1.2f;
    }
    
    float GetNoizeRadius()
    {
        NoiseZone noiseZone = GetComponent<NoiseZone>();
        return noiseZone != null ? noiseZone.radius : 5f;
    }
    
    void Update()
    {
        time += Time.deltaTime;
        
        // Проверяем исчезновение для LootSpot
        if (objectType == ObjectType.LootSpot)
        {
            LootSpot lootSpot = GetComponent<LootSpot>();
            if (lootSpot != null && lootSpot.isLooted)
            {
                HideVisual();
                return;
            }
        }
        
        // Анимация в зависимости от типа
        switch (objectType)
        {
            case ObjectType.LootSpot:
                AnimateLootSpot();
                break;
            case ObjectType.Exit:
                AnimateExit();
                break;
            case ObjectType.Noize:
                AnimateNoize();
                break;
        }
    }
    
    void AnimateLootSpot()
    {
        // Пульсация света
        if (spotLight != null)
        {
            float intensity = 1f + Mathf.Sin(time * 2f) * 0.5f;
            spotLight.intensity = intensity * 2f;
        }
        
        // Вращение
        transform.Rotate(0, 30f * Time.deltaTime, 0);
        
        // Пульсация материала
        float emissionIntensity = 1f + Mathf.Sin(time * 2f) * 0.5f;
        Color emissionColor = lootSpotColor * emissionIntensity;
        visualMaterial.SetColor("_EmissionColor", emissionColor);
    }
    
    void AnimateExit()
    {
        // Пульсация масштаба
        float scaleMultiplier = 1f + Mathf.Sin(time) * 0.1f;
        transform.localScale = Vector3.one * scaleMultiplier;
        
        // Вращение
        transform.Rotate(0, 45f * Time.deltaTime, 0);
        
        // Пульсация материала
        float emissionIntensity = 1f + Mathf.Sin(time * 2f) * 0.3f;
        Color emissionColor = exitColor * emissionIntensity;
        visualMaterial.SetColor("_EmissionColor", emissionColor);
    }
    
    void AnimateNoize()
    {
        // Дрожание
        Vector3 shakeOffset = new Vector3(
            Mathf.Sin(time * 10f) * 0.05f,
            Mathf.Cos(time * 10f * 1.3f) * 0.05f,
            Mathf.Sin(time * 10f * 0.7f) * 0.05f
        );
        transform.position = transform.position + shakeOffset * Time.deltaTime;
        
        // Случайное вращение
        transform.Rotate(
            Random.Range(-90f, 90f) * Time.deltaTime,
            Random.Range(-90f, 90f) * Time.deltaTime,
            Random.Range(-90f, 90f) * Time.deltaTime
        );
        
        // Пульсация материала
        float emissionIntensity = 1f + Mathf.Sin(time * 3f) * 0.5f;
        Color emissionColor = noizeColor * emissionIntensity;
        visualMaterial.SetColor("_EmissionColor", emissionColor);
    }
    
    void HideVisual()
    {
        if (visualObject != null)
            visualObject.SetActive(false);
        if (radiusObject != null)
            radiusObject.SetActive(false);
        if (spotLight != null)
            spotLight.enabled = false;
        enabled = false;
    }
}