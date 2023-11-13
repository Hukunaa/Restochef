using UnityEngine;

public class ChefMaterialManager : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] _bodyMaterialMeshRenderers;
    [SerializeField] private SkinnedMeshRenderer[] _headMaterialMeshRenderers;

    public void AssignBodyMaterial(Material _material)
    {
        foreach (var bodyMaterialMeshRenderer in _bodyMaterialMeshRenderers)
        {
            bodyMaterialMeshRenderer.material = _material;
        }
    }

    public void AssignHeadMaterial(Material _material)
    {
        foreach (var headMaterialMeshRenderer in _headMaterialMeshRenderers)
        {
            headMaterialMeshRenderer.material = _material;
        }
    }
}
