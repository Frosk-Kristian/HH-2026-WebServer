using KokoroSharp;
using KokoroSharp.Core;

namespace HH_2026_WebServer.Services
{
    /// <summary>
    /// Service class for handling text-to-speech.
    /// </summary>
    public class TtsService : IDisposable
    {
        /// <summary>
        /// WAV synthesizer
        /// </summary>
        private readonly KokoroWavSynthesizer _synthesizer;
        /// <summary>
        /// Voice pack used for text to speech
        /// </summary>
        private readonly KokoroVoice _voice;

        public TtsService()
        {
            _synthesizer = KokoroWavSynthesizer.LoadModel();
            _voice = KokoroVoiceManager.GetVoice("bm_lewis");
        }

        public void Dispose() => _synthesizer.Dispose();

        /// <summary>
        /// Generates audio bytes from a given string
        /// </summary>
        /// <param name="text">String text to convert to speech</param>
        /// <returns>Audio bytes representing the synthesized speech</returns>
        /// <exception cref="ArgumentNullException">If text is null or empty</exception>
        public async Task<byte[]> Generate(string text)
        {
            if (string.IsNullOrEmpty(text)) {
                throw new ArgumentNullException(nameof(text));
            }

            byte[] audioBytes = await _synthesizer.SynthesizeAsync(text, _voice);

            return audioBytes;
        }
    }
}
