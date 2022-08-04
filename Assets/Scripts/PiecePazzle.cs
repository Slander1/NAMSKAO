using UnityEngine;

public enum NamePos
{
	CORNER = 0,
	EDGE = 1,
	CENTER = 2
}

public class PiecePazzle : MonoBehaviour
{
	[SerializeField] private NamePos namePos;
	[SerializeField] private int[] tipsPiece;
}

