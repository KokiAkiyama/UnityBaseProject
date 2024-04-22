using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damage
{
    public enum DamageType
    {
        Physical,
        Magic
    }
    /// <summary>
    /// �_���[�W�̒ʒm�f�[�^
    /// </summary>
    public class DamageParam
    {
        public int DamageValue;//�_���[�W�l
        public DamageType damageType;
        public float HitStopDuration;//�q�b�g�X�g�b�v����
        public Vector3 Blow;    //�Ԃ��Ƃ΂��x�N�g��
        public Vector3 HitPosition;//�q�b�g���W
        public void Reset()
        {
            DamageValue = default;
            HitStopDuration = default;
            Blow = default;
            HitPosition= default;
        }
        //�I�u�W�F�N�g�v�[��
        public static UnityEngine.Pool.ObjectPool<DamageParam> s_pool=new(
            createFunc: ()=>new DamageParam(),//�N���X�쐬������
            actionOnGet: target => { target.Reset(); },//Pool����擾���̏���
            actionOnDestroy: target => {  },   //Pool�֖߂����̏���
            collectionCheck: true,           //�d���`�F�b�N
            defaultCapacity: 100,            //�����̌�
            maxSize: 1000                       //�ő��
            );
    }

    public class DamageReply
    {
        //public bool IsHit;//�q�b�g������
        [System.Flags]//�����`�F�b�N��
        public enum ResultTypes
        {
            Hit  =1 << 0,
            Guard=1 << 1,
        }
        public ResultTypes ResultType;
        public int ActualDamageValue;//���ۂɗ^�����_���[�W�l
        public void Reset()
        {
            ResultType = 0;
            ActualDamageValue = 0;
        }
        //�I�u�W�F�N�g�v�[��
        public static UnityEngine.Pool.ObjectPool<DamageReply> s_pool = new(
            createFunc: () => new DamageReply(),//�N���X�쐬������
            actionOnGet: target => { target.Reset(); },//Pool����擾���̏���
            actionOnDestroy: target => { },   //Pool�֖߂����̏���
            collectionCheck: true,           //�d���`�F�b�N
            defaultCapacity: 100,            //�����̌�
            maxSize: 1000                       //�ő��
            );
    }
    /// <summary>
    /// �_���[�W�֌W�̑���
    /// </summary>
    public interface IDamageApplicable
    {
        /// <summary>
        /// �_���[�W�̓K�p
        /// </summary>
        /// <param name="param">�^����_���[�W�̏ڍ�</param>
        /// <param name="rep">���ʎ�M�p</param>
        void ApplyDamege(DamageParam damageParam,DamageReply rep);
        /// <summary>
        /// �e�ւ̃q�b�g�ʒm
        /// </summary>
        /// <param name="param"></param>
        /// <param name="rep"></param>
        void ApplyHit(DamageParam damageParam,DamageReply rep);
    }
    /// <summary>
    /// �q�b�g�ς݃��X�g
    /// </summary>
    public class HitList
    {
        //�q�b�g�����z�̏��
        public class Node
        {
            public MainObjectData Targrt { get; set; }
            //�c�莞��
            public float RemainingTime;
        }
        //�������X�g
        Dictionary<MainObjectData, Node> _objectDic = new();
        List<MainObjectData> deleteList = new(10);
        public void Clear()
        {
            _objectDic.Clear();
        }

        public bool Exist(MainObjectData target)
        {
            if(_objectDic.TryGetValue(target, out Node node))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// �q�b�g�ς݂Ƃ��ēo�^
        /// </summary>
        /// <param name="target">�o�^����z</param>
        /// <param name="duration">���������܂ł̎���</param>
        public void Register(MainObjectData target,float duration)
        {
            //���ɑ��݂���H
           if(_objectDic.TryGetValue(target,out var exist))
           {
                exist.RemainingTime = duration;
           }
           else
           {
                Node node= new Node();
                node.Targrt = target;
                node.RemainingTime = duration;
                _objectDic.Add(target, node);
           }

        }
        

        public void AdvanceTime()
        {
            
            //���Ԃ�i�߂�
            foreach(var node in _objectDic.Values)
            {
                //���Ԃ�i�߂�
                node.RemainingTime -= Time.deltaTime;
            
                if(node.RemainingTime <= 0f)
                {
                    //�폜�\��
                    deleteList.Add(node.Targrt);
                }
            }
            //�폜����
            //UnityEngine.Pool.LinkedPool<MainObjectData>()
            foreach(var target in deleteList)
            {
                _objectDic.Remove(target);
            }
            deleteList.Clear();
        }
    }

}
