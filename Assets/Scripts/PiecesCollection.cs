using System;
using System.Collections.Generic;
using UnityEngine;

public class PiecesCollection
{
	private List<string> _pieceNamePos = new List<string>{ "center" , "edge", "corner"};
	private List<PiecePazzle> _piacesCollection { get; }


	public PiecesCollection(GameObject[] piecesPrefabs)
	{
		
	}

	//private string[] GetDataPiecePazzle(GameObject piecePrefab)
	//{
	//	var piecePazzleDataInString = piecePrefab.name.Split('_');

	//	if (!_pieceNamePos.Contains(piecePazzleDataInString[0]))
	//		return null;



 //   }
}

