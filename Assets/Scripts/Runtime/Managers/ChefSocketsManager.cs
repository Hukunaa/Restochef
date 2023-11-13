using System.Collections;
using System.Collections.Generic;
using Runtime.Enums;
using UnityEngine;

public class ChefSocketsManager : MonoBehaviour
{
    [SerializeField] private Transform _panSocket;
    [SerializeField] private Transform _knifeSocket;
    [SerializeField] private Transform _ingredientSockets;
    
    private Dictionary<EIngredientSocket, Transform> _ingredients3DSockets;
    
    public void InitializeIngredientSockets()
    {
        _ingredients3DSockets = new Dictionary<EIngredientSocket, Transform>
        {
            [EIngredientSocket.Socket1] = _ingredientSockets.Find("Socket1"),
            [EIngredientSocket.Socket2] = _ingredientSockets.Find("Socket2"),
            [EIngredientSocket.Socket3] = _ingredientSockets.Find("Socket3")
        };
    }

    public Transform GetIngredientSocket(EIngredientSocket _ingredientSocket)
    {
        return _ingredients3DSockets[_ingredientSocket];
    }

    public Transform PanSocket => _panSocket;

    public Transform KnifeSocket => _knifeSocket;

    public Transform IngredientSockets => _ingredientSockets;
}
