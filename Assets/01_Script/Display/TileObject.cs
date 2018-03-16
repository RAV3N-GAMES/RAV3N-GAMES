using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject {
	public GameObject mObject; //{ get;} //이거 변경해줘야하나
	public int mRow;		   //{ get;} //바닥타일의 0을 기준으로
	public int mCol;		   //{ get;} //나중에는 변경가능해야함

	public TileObject(GameObject _Obj, int row, int col)
    {
        mObject = _Obj;
        mRow = row;
        mCol = col;
    }
}
