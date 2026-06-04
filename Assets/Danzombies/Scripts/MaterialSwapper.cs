using UnityEngine;

public class MaterialSwapper : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Material material;
    public Color color;
    public float activeTime = 1f;


    private Material m_originalMaterial;
    private Color m_originalColor;
    private float m_elapsedTime;

    private void Awake()
    {
        m_originalMaterial = spriteRenderer.material;
        m_originalColor = spriteRenderer.color;
    }

    private void OnEnable()
    {
        spriteRenderer.material = material;
        spriteRenderer.color = color;
        m_elapsedTime = 0;
    }

    private void Update()
    {
        m_elapsedTime += Time.deltaTime;
        if(m_elapsedTime > activeTime) enabled = false;
    }

    private void OnDisable()
    {
        spriteRenderer.material = m_originalMaterial;
        spriteRenderer.color = m_originalColor;
    }

}
