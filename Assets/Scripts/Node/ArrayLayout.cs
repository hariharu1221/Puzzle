using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

[System.Serializable]
public class ArrayLayout
{
	public ArrayLayout(int height)
    {
		for(int i = 0; i < height; i++)
        {
			rows.Add(new rowData());
		}
    }

	public ArrayLayout()
    {
		for (int i = 0; i < 9; i++)
		{
			rows.Add(new rowData());
			Debug.Log(rows.Count);
		}
	}

	[System.Serializable]
	public struct rowData
	{
		public int[] row;
	}

	public Grid grid;
	public List<rowData> rows = new List<rowData>(); //Grid of 7x7
}
