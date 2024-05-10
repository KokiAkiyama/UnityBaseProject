using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] CharacterDatabase database;

    [SerializeField] List<CharacterBrain> characters = new List<CharacterBrain>();
    public List<CharacterBrain> Characters=>characters;
    /// <summary>
    /// 各陣営のコントローラーを登録
    /// </summary>
    [SerializeField] DictionaryEx<MainObjectData.GroupIDs,Group> groupDic=new();
    public PlayerController PlayerSelector => groupDic[MainObjectData.GroupIDs.Player] as PlayerController;

    public DictionaryEx<MainObjectData.GroupIDs, Group> GroupDic=>groupDic;

    public CharacterData GetChatacrerData(CharacterIDs id) => database.GetChatacrerData(id);

    public DictionaryEx<MainObjectData.GroupIDs,Color> GroupColorDic=new();

    public void Copy(ref CharacterData copiedData,CharacterIDs id)=>database.Copy(ref copiedData,id);

    void OderByTurnCharacterList()
    {
        characters=characters.OrderByDescending(character=>character.Param.dexterity).ToList();
        GameManager.Instance.TurnManager.CreateTurnList();
    }

    

    public void AddCharacter(CharacterBrain character)
    {
        if(characters.Contains(character)){return;}
        characters.Add(character);
        OderByTurnCharacterList();
    }

    public void RemoveDeadCharacter()
    {
        characters.RemoveAll(character =>
        {
            if (character == null) return true;
            return character.IsDead;
        });
    }
    void Start()
    {
        this.UpdateAsObservable()
        .Subscribe(_=>
        {
            
            characters.RemoveAll(character=>character==null);

        }).AddTo(this);
    }
    /// <summary>
    /// 特定のグループを取得
    /// </summary>
    /// <param name="groupID">捜索するグループ</param>
    /// <param name="result">結果</param>
    public void Search(MainObjectData.GroupIDs groupIDFlg,ref List<CharacterBrain> result)
    {
        if(result==null)
        {
            result=new List<CharacterBrain>();
        }
        
        foreach(CharacterBrain character in characters)
        {
            if(character==null){continue;}
            if(character.IsDead){continue;}
            if(character.MainObjectData.Check(groupIDFlg))
            {
                result.Add(character);
            }
        }
    }
    /// <summary>
    /// 特定のグループを近い順に取得
    /// </summary>
    /// <param name="groupID"></param>
    /// <param name="originCharacter">捜索の中心となるキャラクター(捜索対象外)</param>
    /// <param name="result"></param>
    public void Search(MainObjectData.GroupIDs groupID,CharacterBrain originCharacter,ref List<CharacterBrain> result)
    {
        Search(groupID,ref result);
        result.RemoveAll(character=>character==originCharacter);
        result=result.OrderBy(character=>(character.transform.position-originCharacter.transform.position).magnitude).ToList();
    }
}
