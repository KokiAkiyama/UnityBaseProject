using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DictionaryEx<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[Serializable]
	class Node
	{
		public TKey Key;
		public TValue Value;
	}
	[SerializeField] List<Node> _list;

	// Dictionaryが更新済み？
	bool _updatedDictionary = false;

	// ListからDictionaryへのコピー
	void ListToDic()
	{
		// すでにDictionaryが更新済みならスキップ
		if (_updatedDictionary) { return; }

		// Dicクリア
		Clear();

		foreach (var node in _list)
		{
			base.Add(node.Key, node.Value);
		}

		_updatedDictionary = true;
	}

	public new void Add(TKey key, TValue value)
	{
		ListToDic();
		base.Add(key, value);
	}
	public new bool Remove(TKey key)
	{
		ListToDic();
		return base.Remove(key);
	}

	public new bool ContainsKey(TKey key)
	{
		ListToDic();
		return base.ContainsKey(key);
	}
	public new bool ContainsValue(TValue value)
	{
		ListToDic();
		return base.ContainsValue(value);
	}

	public new KeyCollection Keys
	{
		get
		{
			ListToDic();
			return base.Keys;
		}
	}
	public new ValueCollection Values
	{
		get
		{
			ListToDic();
			return base.Values;
		}
	}

	public new TValue this[TKey key]
	{
		get
		{
			ListToDic();
			return base[key];
		}
		set
		{
			ListToDic();
			base[key] = value;
		}
	}

	public new bool TryGetValue(TKey key, out TValue value)
	{
		ListToDic();
		return base.TryGetValue(key, out value);
	}

	// シリアライズされる直前に、こいつが実行される
	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		// Dictionaryが変更されていないならスキップ
		if (!_updatedDictionary) { return; }

		// Dic -> List
		if (_list == null) { _list = new(); }
		_list.Clear();

		foreach (var pair in this)
		{
			Node node = new();
			node.Key = pair.Key;
			node.Value = pair.Value;
			_list.Add(node);
		}

		_updatedDictionary = false;
	}

	// デシリアライズ後に、こいつが実行される
	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
	}
}
