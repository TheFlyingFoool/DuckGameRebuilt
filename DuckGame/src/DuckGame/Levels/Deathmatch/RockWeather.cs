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

        public static Weather weather => _weather;

        private RockWeatherState GetWeatherState(float time, bool lerp = true)
        {
            if (Program.BirthdayDGR)
            {
                RockWeatherState WH = new RockWeatherState();
                WH.add = Vec3.Zero;
                WH.multiply = Vec3.One;
                WH.sky = new Vec3(0.05f);
                WH.lightOpacity = 0.3F;
                WH.sunGlow = 0;
                WH.sunOpacity = 0;
                return WH;
            }
            RockWeatherState rockWeatherState1 = null;
            RockWeatherState rockWeatherState2 = null;
            float num1 = 0f;
            int index = 0;
            switch (_weather)
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
            float num2 = Maths.NormalizeSection(_timeOfDay, num1 * index, num1 * (index + 1));
            RockWeatherState weatherState = new RockWeatherState();
            if (_lastAppliedState == null)
                _lastAppliedState = rockWeatherState1.Copy();
            if (lerp)
            {
                float amount = 0.001f;
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
            lightOpacity = state.lightOpacity;
            sunPos = state.sunPos;
            sunGlow = state.sunGlow;
            sunOpacity = state.sunOpacity;
            _lastAppliedState = state;
        }

        public RockWeather(RockScoreboard board)
          : base()
        {
            layer = Layer.Foreground;
            _board = board;
            if (_weather == Weather.Snowing)
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

        public void Start() => ApplyWeatherState(GetWeatherState(_timeOfDay, false));

        public BitBuffer NetSerialize()
        {
            BitBuffer bitBuffer = new BitBuffer();
            bitBuffer.Write(_timeOfDay);
            bitBuffer.Write((byte)_weather);
            return bitBuffer;
        }

        public void NetDeserialize(BitBuffer data)
        {
            _timeOfDay = data.ReadFloat();
            _weather = (Weather)data.ReadByte();
        }

        public static void TickWeather()
        {
            _timeOfDay += 6.17284E-06f;
            _weatherTime += 6.17284E-06f;
            if (_weather == Weather.Raining)
                _timeRaining += Maths.IncFrameTimer();
            if (_timeOfDay <= 1f)
                return;
            _timeOfDay = 0f;
        }

        public static void Reset()
        {
            _timeOfDay = Rando.Float(0.35f, 0.42f);
            _weatherTime = 1f;
            _weather = Weather.Sunny;
            alwaysRainbow = false;
            neverRainbow = false;
            DateTime localTime = MonoMain.GetLocalTime();
            if (localTime.Month < 3)
            {
                snowChance = 0.1f;
                if (localTime.Month < 2)
                    snowChance = 0.05f;
                rainChance = 0.006f;
                if (localTime.Month < 2)
                    rainChance = 0.003f;
            }
            else if (localTime.Month > 6)
            {
                snowChance = 0.0001f;
                if (localTime.Month > 7)
                {
                    snowChance = 0.001f;
                    if (localTime.Month > 8)
                    {
                        snowChance = 0.01f;
                        if (localTime.Month > 9)
                        {
                            snowChance = 0.03f;
                            if (localTime.Month > 10)
                            {
                                snowChance = 0.08f;
                                if (localTime.Month == 12)
                                    snowChance = 0.25f;
                            }
                        }
                    }
                }
            }
            if (localTime.Month > 3)
            {
                rainChance = 0.08f;
                if (localTime.Month > 5)
                    rainChance = 0.02f;
                if (localTime.Month > 7)
                    rainChance = 0.005f;
                if (localTime.Month > 8)
                    rainChance = 0.001f;
                if (localTime.Month > 10)
                    rainChance = 0f;
            }
            if (localTime.Month == 12 && Rando.Float(1f) > 0.85f)
                _weather = Weather.Snowing;
            if (localTime.Month == 4 && Rando.Float(1f) > 0.92)
            {
                _weather = Weather.Raining;
                rainChance = 0.2f;
            }
            if (localTime.Month == 12 && localTime.Day == 25)
                _weather = Weather.Snowing;
            if (localTime.Month == 4 && localTime.Day == 20)
            {
                _weather = Weather.Raining;
                neverRainbow = true;
            }
            if (localTime.Month == 7 && Rando.Int(10000) == 1)
            {
                _weather = Weather.Snowing;
                snowChance = 0.1f;
            }
            if (localTime.Month == 3 && localTime.Day == 9)
            {
                _weather = Weather.Sunny;
                alwaysRainbow = true;
                rainChance = 0f;
                snowChance = 0f;
            }
            if (localTime.Month == 10 && localTime.Day == 24)
            {
                _weather = Weather.Sunny;
                alwaysRainbow = true;
                rainChance = 0f;
                snowChance = 0f;
            }
            if (localTime.Year == 2030)
            {
                _weather = Weather.Snowing;
                snowChance = 1f;
                rainChance = 0f;
            }
            if (localTime.Year == 2031 && localTime.Month <= 3)
            {
                _weather = Weather.Raining;
                snowChance = 0f;
                rainChance = 1f;
            }
            if (localTime.Year == 2031 && localTime.Month == 4)
            {
                alwaysRainbow = true;
                sunshowers = 999999f;
            }
            if (localTime.Year != 2031 || localTime.Month <= 4)
                return;
            snowChance = 0f;
            if (_weather != Weather.Snowing)
                return;
            _weather = Weather.Raining;
        }

        public void SetWeather(Weather w)
        {
            _weather = w;
            _weatherTime = 0f;
        }

        public override void Update()
        {
            if (alwaysRainbow)
            {
                rainbowFade = 1f;
                rainbowTime = 1f;
            }
            rainbowFade = Lerp.Float(rainbowFade, rainbowTime > 0f ? 1f : 0f, 1f / 1000f);
            rainbowTime -= Maths.IncFrameTimer();
            if (_weather != Weather.Sunny) rainbowTime -= Maths.IncFrameTimer() * 8f;
            if (rainbowTime < 0f) rainbowTime = 0f;
            if (neverRainbow) rainbowFade = 0f;
            RockWeatherState weatherState = GetWeatherState(_timeOfDay);
            rainbowLight = weatherState.rainbowLight * rainbowFade;
            rainbowLight2 = weatherState.rainbowLight2 * rainbowFade;
            ApplyWeatherState(weatherState);
            _prevWeatherLerp = Lerp.Float(_prevWeatherLerp, 0f, 0.05f);
            if (Network.isServer)
            {
                wait += 0.003f;
                if (wait > 1f)
                {
                    wait = 0f;
                    if (_weatherTime > 0.1f)
                    {
                        if (snowChance > 0f && _weather != Weather.Snowing && Rando.Float(1f) > 1f - snowChance)
                        {
                            _prevWeatherLerp = 1f;
                            sunshowers = 0f;
                            _prevWeather = _weather;
                            _weather = Weather.Snowing;
                            if (Network.isActive)
                                Send.Message(new NMChangeWeather((byte)_weather));
                            _weatherTime = 0f;
                        }
                        if (rainChance > 0f && _weather != Weather.Raining && Rando.Float(1f) > 1f - rainChance)
                        {
                            _prevWeatherLerp = 1f;
                            sunshowers = 0f;
                            _prevWeather = _weather;
                            _weather = Weather.Raining;
                            if (Network.isActive)
                                Send.Message(new NMChangeWeather((byte)_weather));
                            _weatherTime = 0f;
                        }
                        if (_weather != Weather.Sunny && Rando.Float(1f) > 0.98f)
                        {
                            _prevWeatherLerp = 1f;
                            if (_weather == Weather.Raining)
                            {
                                if (_timeRaining > 900f && Rando.Float(1f) > 0.45f || Rando.Float(1f) > 0.95f)
                                    rainbowTime = Rando.Float(30f, 240f);
                                if (Rando.Float(1f) > 0.04f)
                                    sunshowers = Rando.Float(0.1f, 60f);
                            }
                            _timeRaining = 0f;
                            _prevWeather = _weather;
                            _weather = Weather.Sunny;
                            if (Network.isActive)
                                Send.Message(new NMChangeWeather((byte)_weather));
                            _weatherTime = 0f;
                        }
                    }
                }
            }
            sunshowers -= Maths.IncFrameTimer();
            if (sunshowers <= 0f)
                sunshowers = 0f;
            switch (_weather)
            {
                case Weather.Snowing:
                    while (_particleWait <= 0f)
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
                    while (_particleWait <= 0f)
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
                    if (sunshowers <= 0f)
                        break;
                    goto case Weather.Raining;
            }
            List<WeatherParticle> weatherParticleList = new List<WeatherParticle>();
            foreach (WeatherParticle particle in _particles)
            {
                particle.Update();
                if (particle.position.y > 0f)
                    particle.die = true;
                switch (particle)
                {
                    case RainParticle _ when particle.z < 70f && particle.position.y > -62f:
                        particle.die = true;
                        particle.position.y = -58f;
                        break;
                    case RainParticle _ when particle.z < 40f && particle.position.y > -98f:
                        particle.die = true;
                        particle.position.y = -98f;
                        break;
                    case RainParticle _ when particle.z < 25f && particle.position.x > 175f && particle.position.x < 430f && particle.position.y > -362f && particle.position.y < -352f:
                        particle.die = true;
                        particle.position.y = -362f;
                        break;
                }
                if (particle.alpha < 0.01f)
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
