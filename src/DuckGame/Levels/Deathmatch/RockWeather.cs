// Decompiled with JetBrains decompiler
// Type: DuckGame.RockWeather
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class RockWeather : Thing
    {
        private static Weather _weather = Weather.Sunny;
        private Color skyColor = new Color(0.5450981f, 0.8f, 0.972549f);
        private Vec3 winterColor = new Vec3(-0.1f, -0.1f, 0.2f);
        private Vec3 summerColor = new Vec3(0f, 0f, 0f);
        private List<WeatherParticle> _particles = new List<WeatherParticle>();
        private float _particleWait;
        private RockScoreboard _board;
        private Color _skyColor;
        private Vec3 _enviroColor;
        private static float _timeOfDay = 0.25f;
        private static float _weatherTime = 1f;
        private List<RockWeatherState> timeOfDayColorMultMap = new List<RockWeatherState>()
    {
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0.06f),
        multiply = new Vec3(0.98f, 0.98f, 1f),
        sky = new Vec3(0.15f, 0.15f, 0.25f),
        sunPos = new Vec2(0f, 0.6f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 0f,
        rainbowLight = 0f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.1f, 0.1f, 0.06f),
        multiply = new Vec3(0.9f, 0.9f, 1f),
        sky = new Vec3(0.1f, 0.1f, 0.2f),
        sunPos = new Vec2(0f, 0.6f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.1f, 0.1f, 0.06f),
        multiply = new Vec3(0.9f, 0.9f, 1f),
        sky = new Vec3(0.1f, 0.1f, 0.2f),
        sunPos = new Vec2(0f, 0.6f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.1f, 0.1f, 0.06f),
        multiply = new Vec3(0.9f, 0.9f, 1f),
        sky = new Vec3(0.1f, 0.1f, 0.2f),
        sunPos = new Vec2(0f, 0.6f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.1f, 0.1f, 0.06f),
        multiply = new Vec3(0.9f, 0.9f, 1f),
        sky = new Vec3(0.1f, 0.1f, 0.2f),
        sunPos = new Vec2(0f, 0.6f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.08f, 0.05f, 0f),
        multiply = new Vec3(1f, 0.8f, 0.7f),
        sky = new Vec3(0.9803922f, 0.7450981f, 0.5490196f),
        sunPos = new Vec2(0f, 1f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0.25f,
        rainbowLight2 = 0.35f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0f),
        multiply = new Vec3(1f, 1f, 1f),
        sky = new Vec3(0.5450981f, 0.8f, 0.972549f),
        sunPos = new Vec2(0.3f, 1.8f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0.25f,
        rainbowLight2 = 0.35f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0f),
        multiply = new Vec3(1f, 1f, 1f),
        sky = new Vec3(0.5450981f, 0.8f, 0.972549f),
        sunPos = new Vec2(0.5f, 0.9f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0.2f,
        rainbowLight2 = 0.35f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0f),
        multiply = new Vec3(1f, 1f, 1f),
        sky = new Vec3(0.5450981f, 0.8f, 0.972549f),
        sunPos = new Vec2(0.5f, 0.9f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0.2f,
        rainbowLight2 = 0.3f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0f),
        multiply = new Vec3(1f, 1f, 1f),
        sky = new Vec3(0.5450981f, 0.8f, 0.972549f),
        sunPos = new Vec2(0.5f, 0.9f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0.25f,
        rainbowLight2 = 0.35f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.08f, 0.05f, 0f),
        multiply = new Vec3(1f, 0.8f, 0.7f),
        sky = new Vec3(0.7843137f, 0.4705882f, 0.7450981f),
        sunPos = new Vec2(0.6f, 0f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0.35f,
        rainbowLight2 = 0.35f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.08f, 0.05f, 0f),
        multiply = new Vec3(1f, 0.8f, 0.7f),
        sky = new Vec3(0.9803922f, 0.6666667f, 0.4313726f),
        sunPos = new Vec2(0.7f, -0.5f),
        lightOpacity = 0f,
        sunGlow = 0.3f,
        sunOpacity = 1f,
        rainbowLight = 0.15f,
        rainbowLight2 = 0.15f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0f),
        multiply = new Vec3(1f, 1f, 1f),
        sky = new Vec3(0.1f, 0.1f, 0.2f),
        sunPos = new Vec2(0.8f, -1f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0.06f),
        multiply = new Vec3(0.98f, 0.98f, 1f),
        sky = new Vec3(0.15f, 0.15f, 0.25f),
        sunPos = new Vec2(0.9f, -1.2f),
        lightOpacity = 1f,
        sunGlow = -0.2f,
        sunOpacity = 0f,
        rainbowLight = 0f
      }
    };
        private List<RockWeatherState> timeOfDayColorMultMapWinter = new List<RockWeatherState>()
    {
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0.06f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(0.98f, 0.98f, 1f),
        sky = new Vec3(0.15f, 0.15f, 0.25f),
        sunPos = new Vec2(0f, 0.6f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 0f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.1f, 0.1f, 0.06f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(0.9f, 0.9f, 1f),
        sky = new Vec3(0.1f, 0.1f, 0.2f),
        sunPos = new Vec2(0f, 0.6f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 1f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.1f, 0.1f, 0.06f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(0.9f, 0.9f, 1f),
        sky = new Vec3(0.1f, 0.1f, 0.2f),
        sunPos = new Vec2(0f, 0.6f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 1f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.1f, 0.1f, 0.06f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(0.9f, 0.9f, 1f),
        sky = new Vec3(0.1f, 0.1f, 0.2f),
        sunPos = new Vec2(0f, 0.6f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 1f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.1f, 0.1f, 0.06f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(0.9f, 0.9f, 1f),
        sky = new Vec3(0.1f, 0.1f, 0.2f),
        sunPos = new Vec2(0f, 0.6f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 1f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.08f, 0.05f, 0f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(1f, 0.8f, 0.7f),
        sky = new Vec3(0.9803922f, 0.7450981f, 0.5490196f),
        sunPos = new Vec2(0f, 1f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0.25f,
        rainbowLight2 = 0.35f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(1f, 1f, 1f),
        sky = new Vec3(0.5450981f, 0.8f, 0.972549f),
        sunPos = new Vec2(0.3f, 1.8f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0.25f,
        rainbowLight2 = 0.35f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(1f, 1f, 1f),
        sky = new Vec3(0.5450981f, 0.8f, 0.972549f),
        sunPos = new Vec2(0.5f, 0.9f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0.2f,
        rainbowLight2 = 0.35f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(1f, 1f, 1f),
        sky = new Vec3(0.5450981f, 0.8f, 0.972549f),
        sunPos = new Vec2(0.5f, 0.9f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0.2f,
        rainbowLight2 = 0.3f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(1f, 1f, 1f),
        sky = new Vec3(0.5450981f, 0.8f, 0.972549f),
        sunPos = new Vec2(0.5f, 0.9f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0.25f,
        rainbowLight2 = 0.35f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.08f, 0.05f, 0f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(1f, 0.8f, 0.7f),
        sky = new Vec3(0.7843137f, 0.4705882f, 0.7450981f),
        sunPos = new Vec2(0.6f, 0f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 1f,
        rainbowLight = 0.35f,
        rainbowLight2 = 0.35f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.08f, 0.05f, 0f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(1f, 0.8f, 0.7f),
        sky = new Vec3(0.9803922f, 0.6666667f, 0.4313726f),
        sunPos = new Vec2(0.7f, -0.5f),
        lightOpacity = 0f,
        sunGlow = 0.3f,
        sunOpacity = 1f,
        rainbowLight = 0.15f,
        rainbowLight2 = 0.15f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(1f, 1f, 1f),
        sky = new Vec3(0.1f, 0.1f, 0.2f),
        sunPos = new Vec2(0.8f, -1f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 1f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0.06f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(0.98f, 0.98f, 1f),
        sky = new Vec3(0.15f, 0.15f, 0.25f),
        sunPos = new Vec2(0.9f, -1.2f),
        lightOpacity = 1f,
        sunGlow = -0.2f,
        sunOpacity = 0f
      }
    };
        private List<RockWeatherState> timeOfDayColorMultMapRaining = new List<RockWeatherState>()
    {
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0.06f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(0.91f, 0.99f, 0.94f),
        sky = new Vec3(0.15f, 0.15f, 0.25f),
        sunPos = new Vec2(0f, 0.6f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 0f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.1f, 0.1f, 0.06f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(0.91f, 0.99f, 0.94f),
        sky = new Vec3(0.1f, 0.1f, 0.2f),
        sunPos = new Vec2(0f, 0.6f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 1f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.1f, 0.1f, 0.06f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(0.91f, 0.99f, 0.94f),
        sky = new Vec3(0.1f, 0.1f, 0.2f),
        sunPos = new Vec2(0f, 0.6f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 1f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.1f, 0.1f, 0.06f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(0.91f, 0.99f, 0.94f),
        sky = new Vec3(0.1f, 0.1f, 0.2f),
        sunPos = new Vec2(0f, 0.6f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 1f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.1f, 0.1f, 0.06f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(0.9f, 0.9f, 1f),
        sky = new Vec3(0.1f, 0.1f, 0.2f),
        sunPos = new Vec2(0f, 0.6f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 1f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.08f, 0.05f, 0f) + new Vec3(-0.15f, -0.1f, 0.1f),
        multiply = new Vec3(1f, 0.85f, 0.7f),
        sky = new Vec3(0.8627451f, 0.6666667f, 0.5490196f),
        sunPos = new Vec2(0f, 1f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 0.5f,
        rainbowLight = 0.25f,
        rainbowLight2 = 0.35f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0f) + new Vec3(-0.15f, -0.1f, 0.1f),
        multiply = new Vec3(0.89f, 1.05f, 1f),
        sky = new Vec3(0.3647059f, 0.5647059f, 0.6078432f),
        sunPos = new Vec2(0.3f, 1.8f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 0.4f,
        rainbowLight = 0.25f,
        rainbowLight2 = 0.35f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0f) + new Vec3(-0.19f, -0.09f, 0.1f),
        multiply = new Vec3(0.89f, 1.05f, 1f),
        sky = new Vec3(0.3647059f, 0.5647059f, 0.6078432f),
        sunPos = new Vec2(0.5f, 0.9f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 0.4f,
        rainbowLight = 0.2f,
        rainbowLight2 = 0.35f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0f) + new Vec3(-0.2f, -0.1f, 0.07f),
        multiply = new Vec3(0.89f, 1.05f, 1f),
        sky = new Vec3(0.3647059f, 0.5647059f, 0.6078432f),
        sunPos = new Vec2(0.5f, 0.9f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 0.4f,
        rainbowLight = 0.2f,
        rainbowLight2 = 0.3f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0f) + new Vec3(-0.22f, -0.12f, 0f),
        multiply = new Vec3(0.86f, 1.05f, 1f),
        sky = new Vec3(0.3254902f, 0.4470588f, 0.5137255f),
        sunPos = new Vec2(0.5f, 0.9f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 0.4f,
        rainbowLight = 0.25f,
        rainbowLight2 = 0.35f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.08f, 0.05f, 0f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(1f, 0.8f, 0.8f),
        sky = new Vec3(0.5882353f, 0.3921569f, 0.627451f),
        sunPos = new Vec2(0.6f, 0f),
        lightOpacity = 0f,
        sunGlow = 0f,
        sunOpacity = 0.4f,
        rainbowLight = 0.35f,
        rainbowLight2 = 0.35f
      },
      new RockWeatherState()
      {
        add = new Vec3(0.08f, 0.05f, 0f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(1f, 0.8f, 0.7f),
        sky = new Vec3(0.7058824f, 0.5490196f, 0.7058824f),
        sunPos = new Vec2(0.7f, -0.5f),
        lightOpacity = 0f,
        sunGlow = 0.3f,
        sunOpacity = 0.4f,
        rainbowLight = 0.15f,
        rainbowLight2 = 0.15f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(1f, 1f, 1f),
        sky = new Vec3(0.1f, 0.15f, 0.2f),
        sunPos = new Vec2(0.8f, -1f),
        lightOpacity = 1f,
        sunGlow = 0f,
        sunOpacity = 0.3f
      },
      new RockWeatherState()
      {
        add = new Vec3(0f, 0f, 0.06f) + new Vec3(-0.1f, -0.1f, 0.2f),
        multiply = new Vec3(0.91f, 0.99f, 0.94f),
        sky = new Vec3(0.15f, 0.15f, 0.25f),
        sunPos = new Vec2(0.9f, -1.2f),
        lightOpacity = 1f,
        sunGlow = -0.2f,
        sunOpacity = 0f
      }
    };
        public static Weather _prevWeather;
        public static float _prevWeatherLerp = 0f;
        public static float lightOpacity;
        public static float sunGlow;
        public static float sunOpacity = 1f;
        public static Vec2 sunPos;
        private RockWeatherState _lastAppliedState;
        private static float snowChance = 0f;
        private static float rainChance = 0f;
        private static float sunshowers = 0f;
        private float wait;
        public float rainbowLight;
        public float rainbowLight2;
        public static float rainbowFade = 0f;
        public static float rainbowTime = 0f;
        public static float _timeRaining = 0f;
        public static bool alwaysRainbow = false;
        public static bool neverRainbow = false;

        public static Weather weather => RockWeather._weather;

        private RockWeatherState GetWeatherState(float time, bool lerp = true)
        {
            RockWeatherState rockWeatherState1 = null;
            RockWeatherState rockWeatherState2 = null;
            float num1 = 0f;
            int index = 0;
            switch (RockWeather._weather)
            {
                case Weather.Sunny:
                    num1 = 1f / timeOfDayColorMultMap.Count;
                    index = (int)(_timeOfDay * timeOfDayColorMultMap.Count);
                    if (index >= timeOfDayColorMultMap.Count)
                        index = timeOfDayColorMultMap.Count - 1;
                    rockWeatherState1 = timeOfDayColorMultMap[index];
                    rockWeatherState2 = index + 1 <= timeOfDayColorMultMap.Count - 1 ? timeOfDayColorMultMap[index + 1] : timeOfDayColorMultMap[0];
                    break;
                case Weather.Snowing:
                    num1 = 1f / timeOfDayColorMultMapWinter.Count;
                    index = (int)(_timeOfDay * timeOfDayColorMultMapWinter.Count);
                    if (index >= timeOfDayColorMultMapWinter.Count)
                        index = timeOfDayColorMultMapWinter.Count - 1;
                    rockWeatherState1 = timeOfDayColorMultMapWinter[index];
                    rockWeatherState2 = index + 1 <= timeOfDayColorMultMapWinter.Count - 1 ? timeOfDayColorMultMapWinter[index + 1] : timeOfDayColorMultMapWinter[0];
                    break;
                case Weather.Raining:
                    num1 = 1f / timeOfDayColorMultMapRaining.Count;
                    index = (int)(_timeOfDay * timeOfDayColorMultMapRaining.Count);
                    if (index >= timeOfDayColorMultMapRaining.Count)
                        index = timeOfDayColorMultMapRaining.Count - 1;
                    rockWeatherState1 = timeOfDayColorMultMapRaining[index];
                    rockWeatherState2 = index + 1 <= timeOfDayColorMultMapRaining.Count - 1 ? timeOfDayColorMultMapRaining[index + 1] : timeOfDayColorMultMapRaining[0];
                    break;
            }
            float num2 = Maths.NormalizeSection(RockWeather._timeOfDay, num1 * index, num1 * (index + 1));
            RockWeatherState weatherState = new RockWeatherState();
            if (_lastAppliedState == null)
                _lastAppliedState = rockWeatherState1.Copy();
            if (lerp)
            {
                float amount = 1f / 1000f;
                weatherState.add = Lerp.Vec3(_lastAppliedState.add, rockWeatherState1.add + (rockWeatherState2.add - rockWeatherState1.add) * num2, amount);
                weatherState.multiply = Lerp.Vec3(_lastAppliedState.multiply, rockWeatherState1.multiply + (rockWeatherState2.multiply - rockWeatherState1.multiply) * num2, amount);
                weatherState.sky = Lerp.Vec3(_lastAppliedState.sky, rockWeatherState1.sky + (rockWeatherState2.sky - rockWeatherState1.sky) * num2, amount);
                weatherState.lightOpacity = Lerp.Float(_lastAppliedState.lightOpacity, rockWeatherState1.lightOpacity + (rockWeatherState2.lightOpacity - rockWeatherState1.lightOpacity) * num2, amount);
                weatherState.sunPos = Lerp.Vec2(_lastAppliedState.sunPos, rockWeatherState1.sunPos + (rockWeatherState2.sunPos - rockWeatherState1.sunPos) * num2, amount);
                weatherState.sunGlow = Lerp.Float(_lastAppliedState.sunGlow, rockWeatherState1.sunGlow + (rockWeatherState2.sunGlow - rockWeatherState1.sunGlow) * num2, amount);
                weatherState.sunOpacity = Lerp.Float(_lastAppliedState.sunOpacity, rockWeatherState1.sunOpacity + (rockWeatherState2.sunOpacity - rockWeatherState1.sunOpacity) * num2, amount);
                weatherState.rainbowLight = Lerp.Float(_lastAppliedState.rainbowLight, rockWeatherState1.rainbowLight + (rockWeatherState2.rainbowLight - rockWeatherState1.rainbowLight) * num2, amount);
                weatherState.rainbowLight2 = Lerp.Float(_lastAppliedState.rainbowLight2, rockWeatherState1.rainbowLight2 + (rockWeatherState2.rainbowLight2 - rockWeatherState1.rainbowLight2) * num2, amount);
            }
            else
            {
                weatherState.add = rockWeatherState1.add + (rockWeatherState2.add - rockWeatherState1.add) * num2;
                weatherState.multiply = rockWeatherState1.multiply + (rockWeatherState2.multiply - rockWeatherState1.multiply) * num2;
                weatherState.sky = rockWeatherState1.sky + (rockWeatherState2.sky - rockWeatherState1.sky) * num2;
                weatherState.lightOpacity = rockWeatherState1.lightOpacity + (rockWeatherState2.lightOpacity - rockWeatherState1.lightOpacity) * num2;
                weatherState.sunPos = rockWeatherState1.sunPos + (rockWeatherState2.sunPos - rockWeatherState1.sunPos) * num2;
                weatherState.sunGlow = rockWeatherState1.sunGlow + (rockWeatherState2.sunGlow - rockWeatherState1.sunGlow) * num2;
                weatherState.sunOpacity = rockWeatherState1.sunOpacity + (rockWeatherState2.sunOpacity - rockWeatherState1.sunOpacity) * num2;
                weatherState.rainbowLight = rockWeatherState1.rainbowLight + (rockWeatherState2.rainbowLight - rockWeatherState1.rainbowLight) * num2;
                weatherState.rainbowLight2 = rockWeatherState1.rainbowLight2 + (rockWeatherState2.rainbowLight2 - rockWeatherState1.rainbowLight2) * num2;
            }
            _lastAppliedState = weatherState;
            return weatherState;
        }

        private void ApplyWeatherState(RockWeatherState state)
        {
            Layer.Game.colorMul = state.multiply * Layer.Game.fade;
            Layer.Background.colorMul = state.multiply * Layer.Background.fade;
            _board.fieldMulColor = state.multiply * Layer.Game.fade;
            Layer.Game.colorAdd = state.add * Layer.Game.fade;
            Layer.Background.colorAdd = state.add * Layer.Background.fade;
            _board.fieldAddColor = state.add * Layer.Game.fade;
            Level.current.backgroundColor = new Color(state.sky.x, state.sky.y, state.sky.z) * Layer.Game.fade;
            RockWeather.lightOpacity = state.lightOpacity;
            RockWeather.sunPos = state.sunPos;
            RockWeather.sunGlow = state.sunGlow;
            RockWeather.sunOpacity = state.sunOpacity;
            _lastAppliedState = state;
        }

        public RockWeather(RockScoreboard board)
          : base()
        {
            layer = Layer.Foreground;
            _board = board;
            if (RockWeather._weather == Weather.Snowing)
            {
                _skyColor = skyColor;
                _enviroColor = winterColor;
            }
            else
            {
                _skyColor = skyColor;
                _enviroColor = summerColor;
            }
            RainParticle.splash = new SpriteMap("rainSplash", 8, 8);
        }

        public void Start() => ApplyWeatherState(GetWeatherState(RockWeather._timeOfDay, false));

        public BitBuffer NetSerialize()
        {
            BitBuffer bitBuffer = new BitBuffer();
            bitBuffer.Write(RockWeather._timeOfDay);
            bitBuffer.Write((byte)RockWeather._weather);
            return bitBuffer;
        }

        public void NetDeserialize(BitBuffer data)
        {
            RockWeather._timeOfDay = data.ReadFloat();
            RockWeather._weather = (Weather)data.ReadByte();
        }

        public static void TickWeather()
        {
            RockWeather._timeOfDay += 6.17284E-06f;
            RockWeather._weatherTime += 6.17284E-06f;
            if (RockWeather._weather == Weather.Raining)
                RockWeather._timeRaining += Maths.IncFrameTimer();
            if (_timeOfDay <= 1.0)
                return;
            RockWeather._timeOfDay = 0f;
        }

        public static void Reset()
        {
            RockWeather._timeOfDay = Rando.Float(0.35f, 0.42f);
            RockWeather._weatherTime = 1f;
            RockWeather._weather = Weather.Sunny;
            RockWeather.alwaysRainbow = false;
            RockWeather.neverRainbow = false;
            DateTime localTime = MonoMain.GetLocalTime();
            if (localTime.Month < 3)
            {
                RockWeather.snowChance = 0.1f;
                if (localTime.Month < 2)
                    RockWeather.snowChance = 0.05f;
                RockWeather.rainChance = 3f / 500f;
                if (localTime.Month < 2)
                    RockWeather.rainChance = 3f / 1000f;
            }
            else if (localTime.Month > 6)
            {
                RockWeather.snowChance = 0.0001f;
                if (localTime.Month > 7)
                {
                    RockWeather.snowChance = 1f / 1000f;
                    if (localTime.Month > 8)
                    {
                        RockWeather.snowChance = 0.01f;
                        if (localTime.Month > 9)
                        {
                            RockWeather.snowChance = 0.03f;
                            if (localTime.Month > 10)
                            {
                                RockWeather.snowChance = 0.08f;
                                if (localTime.Month == 12)
                                    RockWeather.snowChance = 0.25f;
                            }
                        }
                    }
                }
            }
            if (localTime.Month > 3)
            {
                RockWeather.rainChance = 0.08f;
                if (localTime.Month > 5)
                    RockWeather.rainChance = 0.02f;
                if (localTime.Month > 7)
                    RockWeather.rainChance = 0.005f;
                if (localTime.Month > 8)
                    RockWeather.rainChance = 1f / 1000f;
                if (localTime.Month > 10)
                    RockWeather.rainChance = 0f;
            }
            if (localTime.Month == 12 && Rando.Float(1f) > 0.850000023841858)
                RockWeather._weather = Weather.Snowing;
            if (localTime.Month == 4 && Rando.Float(1f) > 0.920000016689301)
            {
                RockWeather._weather = Weather.Raining;
                RockWeather.rainChance = 0.2f;
            }
            if (localTime.Month == 12 && localTime.Day == 25)
                RockWeather._weather = Weather.Snowing;
            if (localTime.Month == 4 && localTime.Day == 20)
            {
                RockWeather._weather = Weather.Raining;
                RockWeather.neverRainbow = true;
            }
            if (localTime.Month == 7 && Rando.Int(10000) == 1)
            {
                RockWeather._weather = Weather.Snowing;
                RockWeather.snowChance = 0.1f;
            }
            if (localTime.Month == 3 && localTime.Day == 9)
            {
                RockWeather._weather = Weather.Sunny;
                RockWeather.alwaysRainbow = true;
                RockWeather.rainChance = 0f;
                RockWeather.snowChance = 0f;
            }
            if (localTime.Month == 10 && localTime.Day == 24)
            {
                RockWeather._weather = Weather.Sunny;
                RockWeather.alwaysRainbow = true;
                RockWeather.rainChance = 0f;
                RockWeather.snowChance = 0f;
            }
            if (localTime.Year == 2030)
            {
                RockWeather._weather = Weather.Snowing;
                RockWeather.snowChance = 1f;
                RockWeather.rainChance = 0f;
            }
            if (localTime.Year == 2031 && localTime.Month <= 3)
            {
                RockWeather._weather = Weather.Raining;
                RockWeather.snowChance = 0f;
                RockWeather.rainChance = 1f;
            }
            if (localTime.Year == 2031 && localTime.Month == 4)
            {
                RockWeather.alwaysRainbow = true;
                RockWeather.sunshowers = 999999f;
            }
            if (localTime.Year != 2031 || localTime.Month <= 4)
                return;
            RockWeather.snowChance = 0f;
            if (RockWeather._weather != Weather.Snowing)
                return;
            RockWeather._weather = Weather.Raining;
        }

        public void SetWeather(Weather w)
        {
            RockWeather._weather = w;
            RockWeather._weatherTime = 0f;
        }

        public override void Update()
        {
            if (RockWeather.alwaysRainbow)
            {
                RockWeather.rainbowFade = 1f;
                RockWeather.rainbowTime = 1f;
            }
            RockWeather.rainbowFade = Lerp.Float(RockWeather.rainbowFade, rainbowTime > 0.0 ? 1f : 0f, 1f / 1000f);
            RockWeather.rainbowTime -= Maths.IncFrameTimer();
            if (RockWeather._weather != Weather.Sunny)
                RockWeather.rainbowTime -= Maths.IncFrameTimer() * 8f;
            if (rainbowTime < 0.0)
                RockWeather.rainbowTime = 0f;
            if (RockWeather.neverRainbow)
                RockWeather.rainbowFade = 0f;
            RockWeatherState weatherState = GetWeatherState(RockWeather._timeOfDay);
            rainbowLight = weatherState.rainbowLight * RockWeather.rainbowFade;
            rainbowLight2 = weatherState.rainbowLight2 * RockWeather.rainbowFade;
            ApplyWeatherState(weatherState);
            RockWeather._prevWeatherLerp = Lerp.Float(RockWeather._prevWeatherLerp, 0f, 0.05f);
            if (Network.isServer)
            {
                wait += 3f / 1000f;
                if (wait > 1.0)
                {
                    wait = 0f;
                    if (_weatherTime > 0.100000001490116)
                    {
                        if (snowChance > 0.0 && RockWeather._weather != Weather.Snowing && Rando.Float(1f) > 1.0 - snowChance)
                        {
                            RockWeather._prevWeatherLerp = 1f;
                            RockWeather.sunshowers = 0f;
                            RockWeather._prevWeather = RockWeather._weather;
                            RockWeather._weather = Weather.Snowing;
                            if (Network.isActive)
                                Send.Message(new NMChangeWeather((byte)RockWeather._weather));
                            RockWeather._weatherTime = 0f;
                        }
                        if (rainChance > 0.0 && RockWeather._weather != Weather.Raining && Rando.Float(1f) > 1.0 - rainChance)
                        {
                            RockWeather._prevWeatherLerp = 1f;
                            RockWeather.sunshowers = 0f;
                            RockWeather._prevWeather = RockWeather._weather;
                            RockWeather._weather = Weather.Raining;
                            if (Network.isActive)
                                Send.Message(new NMChangeWeather((byte)RockWeather._weather));
                            RockWeather._weatherTime = 0f;
                        }
                        if (RockWeather._weather != Weather.Sunny && Rando.Float(1f) > 0.980000019073486)
                        {
                            RockWeather._prevWeatherLerp = 1f;
                            if (RockWeather._weather == Weather.Raining)
                            {
                                if (_timeRaining > 900.0 && Rando.Float(1f) > 0.449999988079071 || Rando.Float(1f) > 0.949999988079071)
                                    RockWeather.rainbowTime = Rando.Float(30f, 240f);
                                if (Rando.Float(1f) > 0.400000005960464)
                                    RockWeather.sunshowers = Rando.Float(0.1f, 60f);
                            }
                            RockWeather._timeRaining = 0f;
                            RockWeather._prevWeather = RockWeather._weather;
                            RockWeather._weather = Weather.Sunny;
                            if (Network.isActive)
                                Send.Message(new NMChangeWeather((byte)RockWeather._weather));
                            RockWeather._weatherTime = 0f;
                        }
                    }
                }
            }
            RockWeather.sunshowers -= Maths.IncFrameTimer();
            if (sunshowers <= 0.0)
                RockWeather.sunshowers = 0f;
            switch (RockWeather._weather)
            {
                case Weather.Snowing:
                    while (_particleWait <= 0.0)
                    {
                        ++_particleWait;
                        SnowParticle snowParticle = new SnowParticle(new Vec2(Rando.Float(-100f, 400f), Rando.Float(-500f, -550f)))
                        {
                            z = Rando.Float(0f, 200f)
                        };
                        _particles.Add(snowParticle);
                    }
                    _particleWait -= 0.5f;
                    break;
                case Weather.Raining:
                    while (_particleWait <= 0.0)
                    {
                        ++_particleWait;
                        RainParticle rainParticle = new RainParticle(new Vec2(Rando.Float(-100f, 900f), Rando.Float(-500f, -550f)))
                        {
                            z = Rando.Float(0f, 200f)
                        };
                        _particles.Add(rainParticle);
                    }
                    --_particleWait;
                    break;
                default:
                    if (sunshowers <= 0.0)
                        break;
                    goto case Weather.Raining;
            }
            List<WeatherParticle> weatherParticleList = new List<WeatherParticle>();
            foreach (WeatherParticle particle in _particles)
            {
                particle.Update();
                if (particle.position.y > 0.0)
                    particle.die = true;
                switch (particle)
                {
                    case RainParticle _ when particle.z < 70.0 && particle.position.y > -62.0:
                        particle.die = true;
                        particle.position.y = -58f;
                        break;
                    case RainParticle _ when particle.z < 40.0 && particle.position.y > -98.0:
                        particle.die = true;
                        particle.position.y = -98f;
                        break;
                    case RainParticle _ when particle.z < 25.0 && particle.position.x > 175.0 && particle.position.x < 430.0 && particle.position.y > -362.0 && particle.position.y < -352.0:
                        particle.die = true;
                        particle.position.y = -362f;
                        break;
                }
                if (particle.alpha < 0.00999999977648258)
                    weatherParticleList.Add(particle);
            }
            foreach (WeatherParticle weatherParticle in weatherParticleList)
                _particles.Remove(weatherParticle);
        }

        public override void Draw()
        {
            if (RockScoreboard.drawingLighting)
                return;
            foreach (WeatherParticle particle in _particles)
                particle.Draw();
        }
    }
}
