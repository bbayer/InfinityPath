using System.Collections;
using BBCommon;
using UnityEngine;

public class SoundManager:BBCommon.Singleton<SoundManager>
{



	private AudioSource[] audioSources;
	public AudioClip m_crashClip;
	public AudioClip m_scoreClip;
	public AudioClip m_coinClip;
	public AudioClip m_clickClip;
	public AudioClip m_jumpClip;
	public AudioClip m_RewardClip;
	public AudioClip m_ErrorClip;
	public AudioClip m_BestScore;

	public AudioClip[] m_turnClip;
	public AudioClip m_musicClip;

	private int m_turnToggle;


	public void Awake ()
	{
		audioSources = GetComponents<AudioSource>();
	}

	//Sound 

	public void MuteMusic(){
		audioSources[1].mute = true;
	}


	public void UnmuteMusic(){
		audioSources[1].mute = false;
	}

	public void MusicStart(){
		audioSources[1].loop = true;
		audioSources[1].clip = m_musicClip;
		audioSources[1].Play();
	}

	public void Gong(){
		audioSources[1].PlayOneShot(m_musicClip);
	}

	public void MusicStop(){
		audioSources[1].loop = true;
		audioSources[1].clip = m_musicClip;
		audioSources[1].Play();
	}

	public void Crash(){
		audioSources[0].Stop();
		audioSources[0].PlayOneShot(m_crashClip);
	}

	public void Coin(){
		audioSources[0].PlayOneShot(m_coinClip);
	}

	public void Score(){
		audioSources[0].PlayOneShot(m_scoreClip);
	}

	public void Click(){
		audioSources[0].PlayOneShot(m_clickClip);

	}

	public void Jump (){
		audioSources[0].PlayOneShot(m_jumpClip);
	}

	public void Turn (){
		m_turnToggle++;

		audioSources[0].PlayOneShot(m_turnClip[m_turnToggle % m_turnClip.Length]);
	}

	public void Reward(){
		audioSources[0].PlayOneShot(m_RewardClip);

	}

	public void Error(){
		audioSources[0].PlayOneShot(m_ErrorClip);

	}
	public void BestScore(){
		audioSources[0].PlayOneShot(m_BestScore);

	}

}


