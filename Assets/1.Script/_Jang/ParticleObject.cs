using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : MonoBehaviour {

	ParticleSystem particle;
	private void Awake()
	{
		particle = GetComponent<ParticleSystem>();
	}
	private void OnEnable()
	{
		particle.Play();
	}
	private void Update()
	{
		if (!particle.isPlaying)
		{
			PoolManager.current.PushParticle(gameObject);
		}
	}
}




