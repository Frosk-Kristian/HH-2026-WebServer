using KokoroSharp;
using KokoroSharp.Core;
using NAudio.Wave;

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
        /// Generates audio bytes from a given string, converts to .wav format and returns without ever saving the file to disk.
        /// </summary>
        /// <param name="text">String text to convert to speech</param>
        /// <returns>Byte array .wav file</returns>
        /// <exception cref="ArgumentNullException">If text is null or empty</exception>
        public async Task<byte[]> Generate(string text)
        {
            if (string.IsNullOrEmpty(text)) {
                throw new ArgumentNullException(nameof(text));
            }

            // Raw audio samples, NOT a completed .wav file yet
            byte[] audioBytes = await _synthesizer.SynthesizeAsync(text, _voice);

            // Converts raw audio samples to a .wav file format and returns the resulting byte array
            using var memoryStream = new MemoryStream();
            using (var writer = new WaveFileWriter(memoryStream, KokoroPlayback.waveFormat))
            {
                writer.Write(audioBytes, 0, audioBytes.Length);
            }

            return memoryStream.ToArray();
        }
    }
}
