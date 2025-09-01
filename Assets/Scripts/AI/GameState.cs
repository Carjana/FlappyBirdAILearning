

using System;
using Newtonsoft.Json;
using UnityEngine;

namespace AI
{
    [Serializable]
    public struct GameState
    {
        [JsonConverter(typeof(Vector2Converter))]public Vector2 NearestObstacle;
    }
}