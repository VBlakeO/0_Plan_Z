using UnityEngine;

public class RandomRenderMaterialColor : RendererMaterialArrayColorSet
{
    public RandomColor[] randomColor = new RandomColor[1];

    protected override void OnValidate()
    {
        
    }

    public override void UpadateColor()
    {
        for (int i = 0; i < colors.Length; i++)
        {
            Renderer renderer = GetComponent<Renderer>();
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

            renderer.GetPropertyBlock(propertyBlock, i);
            propertyBlock.SetColor(property, randomColor[i].colors[Random.Range(0, randomColor[i].colors.Length)]);

            // if (changeTexture)
            //     propertyBlock.SetTexture(textureProperty, texture);
                
            renderer.SetPropertyBlock(propertyBlock, i);
        }
    }
}

[System.Serializable]
public class RandomColor
{
    public Color[] colors = new Color[1];
}