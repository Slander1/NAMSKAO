using UnityEngine;

public enum NamePos
{
	CORNER = 0,
	EDGE = 1,
	CENTER = 2
}

public class PiecePazzle : MonoBehaviour
{
	[field: SerializeField] public PieceData PieceData { get; set; }
}

[System.Serializable]
public class PieceData
{
	public NamePos namePos;
	public PossibleTips[] tipsPiece;

	public PieceData(NamePos namePos, PossibleTips[] tipsPiece)
	{
		this.namePos = namePos;
		this.tipsPiece = tipsPiece;
	}
}

