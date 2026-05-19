using UnityEngine;

public class RendererMaterialArrayColorSet : MonoBehaviour
{
    public string property = "_MainColor";
    public Color[] colors = new Color[1];


    protected virtual void OnValidate()
    {
        UpadateColor();
    }
    private void Awake()
    {
        UpadateColor();
    }

    public virtual void UpadateColor()
    {
        for (int i = 0; i < colors.Length; i++)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            Renderer renderer = GetComponent<Renderer>();
            renderer.GetPropertyBlock(propertyBlock, i);
            propertyBlock.SetColor(property, colors[i]);

            // if (changeTexture)
            //     propertyBlock.SetTexture(textureProperty, texture);
                
            renderer.SetPropertyBlock(propertyBlock, i);
        }
    }
}