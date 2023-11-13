using Runtime.Audio.AudioData;
using UnityEngine;

namespace Audio.SFXPlayers.CharacterSFXPlayers
{
    public class ConnectionInterfaceAudio : SFXAudioPlayer
    {
        [SerializeField] private AudioCueSO interfaceMove;
        [SerializeField] private AudioCueSO interfaceConnect;
        [SerializeField] private AudioCueSO interfaceConnected;
        [SerializeField] private AudioCueSO interfaceDisconnect;

        private AudioCueKey m_currentAudioKey;
        private AudioCueKey m_interfaceMoveAudioKey;

        public void PlayInterfaceMove()
        {
            StopInterfaceMove();
            m_interfaceMoveAudioKey = PlayAudio(interfaceMove);
        }

        public void PlayInterfaceConnecting()
        {
            m_currentAudioKey = PlayAudio(interfaceConnect);
        }

        public void PlayInterfaceConnected()
        {
            m_currentAudioKey = PlayAudio(interfaceConnected);
        }

        public void PlayInterfaceDisconnect()
        {
            m_currentAudioKey = PlayAudio(interfaceDisconnect);
        }

        public void StopInterfaceMove()
        {
            StopAudio(m_interfaceMoveAudioKey);
        }
    }
}