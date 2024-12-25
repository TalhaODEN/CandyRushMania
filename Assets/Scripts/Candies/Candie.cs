using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
public class Candie : MonoBehaviour
{
    private Board board;
    public int row, column;
    private void Start()
    {
        board = FindObjectOfType<Board>();
    }   
}
