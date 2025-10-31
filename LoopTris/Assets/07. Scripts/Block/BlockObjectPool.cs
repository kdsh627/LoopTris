using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BlockObjectPool : MonoBehaviour
{
    private IObjectResolver _resolver;
    private GameObject _block;

    [Inject]
    public void Construct(IObjectResolver resolver)
    {
        _resolver = resolver;
    }

    public void BlockSpawn()
    {
        GameObject go =_resolver.Instantiate(_block);
    }
}
