using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraScalar : MonoBehaviour
{
    private Board board;
    [SerializeField] private float cameraOffset;
    [SerializeField] private float aspectRation;
    [SerializeField] private float padding;
    [SerializeField] private float yOffset;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            RepositionCamera(board.width - 1, board.height - 1);
        }
    }

    void RepositionCamera(float x, float y)
    {
        Vector3 tempPos = new Vector3(x/2, y/2 + yOffset, cameraOffset);
        transform.position = tempPos;
        if (board.width >= board.height)
        {
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRation;
        }
        else
        {
            Camera.main.orthographicSize = board.height + padding;
        }
        
    }

}
