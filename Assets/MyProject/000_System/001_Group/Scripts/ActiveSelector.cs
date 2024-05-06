using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 行動中のキャラクターを制御するコンポーネント
/// </summary>
public class ActiveSelector : MonoBehaviour
{
    /// <summary>
    /// 現在アクション途中のキャラクター一覧
    /// (要素が存在する時は新たな指示を与えることができない)
    /// </summary>
    [SerializeField]
    List<CharacterBrain> activeControls=new();

    public List<CharacterBrain> ActiveControls=>activeControls;

    public void AddActveControl(CharacterBrain character)
    {
        if (activeControls.Contains(character)) { return; }
        activeControls.Add(character);
    }

    public void EndActveControl(CharacterBrain character)
    {
        if(activeControls.Contains(character)==false){return;}
        activeControls.Remove(character);
    }

    public bool CanControl=>activeControls.Count<=0;


    // Update is called once per frame
    void Update()
    {
        activeControls.RemoveAll(character=>character==null);
        
    }

}
