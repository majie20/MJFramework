// created by StarkMini

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;

namespace StarkMini.Examples {
	public class AddressableReference : MonoBehaviour {
		public AssetReference m_AssertPrefab;

		// Use this for initialization
		void Start() {
			m_AssertPrefab.InstantiateAsync(transform.position, transform.rotation);
		}
	}
}
