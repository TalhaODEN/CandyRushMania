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
        /*for (int x = 0; x < board.AllCandies.GetLength(0); x++)
        {
            for(int y = 0; y < board.AllCandies.GetLength(1); y++)
            {
                Transform tileTransform = board.AllCandies[x, y].transform;
                if(tileTransform.position == transform.position)
                {
                    row = x;
                    column = y;
                    return;
                }
            }
        }*/
    }
    
}
