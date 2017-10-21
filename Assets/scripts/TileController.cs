using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{

    public int indexI = -1;
    public int indexJ = -1;
    private TileState tileState = TileState.Unknown;
    private GameObject transparentTile = null;
    public bool actAsGrassTile;

    public enum TileState
    {
        Unknown,
        Visible,
        Memorized,
        Invisible
    }

    public enum RenderingMode
    {
        Transparent,
        Cutout
    }

    public TileController(int indexI, int indexJ)
    {
        this.indexI = indexI;
        this.indexJ = indexJ;
    }

    public void setState(TileState tileState)
    {
        switch (tileState)
        {
            case TileState.Visible:
                if (transparentTile != null)
                {
                    Destroy(transparentTile);
                    transparentTile = null;
                }

                foreach (Renderer renderer in this.GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = true;
                }

                foreach(Collider collider in this.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = true;
                }
                break;
            case TileState.Memorized:
                foreach (Renderer renderer in this.GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = true;
                }

                foreach (Collider collider in this.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = true;
                }

                if (transparentTile == null)
                {
                    transparentTile = Instantiate(this.gameObject, this.transform.position, transform.rotation, transform.parent);

                    transparentTile.GetComponent<TileController>().actAsGrassTile = true;
                }

                setTileMaterialRenderingMode(transparentTile.GetComponent<TileController>(), RenderingMode.Transparent);

                foreach (Renderer renderer in this.GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = false;
                }

                if (this is FlowerTileController || this is HiveTileController)
                {
                    foreach (Collider collider in this.GetComponentsInChildren<Collider>())
                    {
                        collider.enabled = false;
                    }
                }

                break;
            case TileState.Invisible:
                if (transparentTile != null)
                {
                    Destroy(transparentTile);
                    transparentTile = null;
                }

                foreach (Renderer renderer in this.GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = false;
                }


                if(this is FlowerTileController || this is HiveTileController)
                {
                    foreach (Collider collider in this.GetComponentsInChildren<Collider>())
                    {
                        collider.enabled = false;
                    }
                }

                break;
        }

        this.tileState = tileState;
    }

    private void setTileMaterialRenderingMode(TileController targetTileController, RenderingMode renderingMode)
    {
        switch (renderingMode)
        {
            case RenderingMode.Cutout:
                break;
            case RenderingMode.Transparent:
                if (targetTileController is FlowerTileController)
                {
                    foreach (Renderer thisRenderer in targetTileController.GetComponentsInChildren<Renderer>())
                    {
                        thisRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        thisRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        thisRenderer.material.SetInt("_ZWrite", 0);
                        thisRenderer.material.DisableKeyword("_ALPHATEST_ON");
                        thisRenderer.material.DisableKeyword("_ALPHABLEND_ON");
                        thisRenderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        thisRenderer.material.renderQueue = 3000;
                        if(thisRenderer.material.HasProperty("_Color"))
                            thisRenderer.material.color = new Color(thisRenderer.material.color.r, thisRenderer.material.color.g, thisRenderer.material.color.b, 0.3f);
                    }
                } else if(targetTileController is GrassTileController)
                {
                    foreach (Renderer thisRenderer in targetTileController.GetComponentsInChildren<Renderer>())
                    {
                        thisRenderer.material.color = new Color(thisRenderer.material.color.r, thisRenderer.material.color.g, thisRenderer.material.color.b, 0.2f);
                    }
                } else if(targetTileController is HiveTileController)
                {
                    foreach (Renderer thisRenderer in targetTileController.GetComponentsInChildren<Renderer>())
                    {
                        thisRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        thisRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        thisRenderer.material.SetInt("_ZWrite", 0);
                        thisRenderer.material.DisableKeyword("_ALPHATEST_ON");
                        thisRenderer.material.DisableKeyword("_ALPHABLEND_ON");
                        thisRenderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        thisRenderer.material.renderQueue = 3000;
                        if (thisRenderer.material.HasProperty("_Color"))
                            thisRenderer.material.color = new Color(thisRenderer.material.color.r, thisRenderer.material.color.g, thisRenderer.material.color.b, 0.3f);
                    }
                }
                break;
        }
    }

    public TileState getState()
    {
        return this.tileState;
    }
}
