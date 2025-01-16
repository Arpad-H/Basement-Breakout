/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#if UNITY_ANDROID && !UNITY_EDITOR
#define JNI_AVAILABLE
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class OVRMetricsToolSDK : MonoBehaviour
{

    [Serializable]
    public struct MetricsSnapshot
    {
        public long time;

        public int available_memory_MB;
        public int app_pss_MB;
        public int battery_level_percentage;
        public int battery_temperature_celcius;
        public int battery_current_now_milliamps;
        public int sensor_temperature_celcius;
        public int power_current;
        public int power_level_state;
        public int power_voltage;
        public int power_wattage;
        public int cpu_level;
        public int gpu_level;
        public int cpu_frequency_MHz;
        public int gpu_frequency_MHz;
        public int mem_frequency_MHz;
        public int minimum_vsyncs;
        public int extra_latency_mode;
        public int average_frame_rate;
        public int display_refresh_rate;
        public int average_prediction_milliseconds;
        public int screen_tear_count;
        public int early_frame_count;
        public int stale_frame_count;
        public int maximum_rotational_speed_degrees_per_second;
        public int foveation_level;
        public int eye_buffer_width;
        public int eye_buffer_height;
        public int app_gpu_time_microseconds;
        public int timewarp_gpu_time_microseconds;
        public int guardian_gpu_time_microseconds;
        public int cpu_utilization_percentage;
        public int cpu_utilization_percentage_core0;
        public int cpu_utilization_percentage_core1;
        public int cpu_utilization_percentage_core2;
        public int cpu_utilization_percentage_core3;
        public int cpu_utilization_percentage_core4;
        public int cpu_utilization_percentage_core5;
        public int cpu_utilization_percentage_core6;
        public int cpu_utilization_percentage_core7;
        public int gpu_utilization_percentage;
        public int spacewarp_motion_vector_type;
        public int spacewarped_frames_per_second;
        public int app_vss_MB;
        public int app_rss_MB;
        public int app_dalvik_pss_MB;
        public int app_private_dirty_MB;
        public int app_private_clean_MB;
        public int app_uss_MB;
        public int stale_frames_consecutive;
        public int avg_vertices_per_frame;
        public int avg_fill_percentage;
        public int avg_inst_per_frag;
        public int avg_inst_per_vert;
        public int avg_textures_per_frag;
        public int percent_time_shading_frags;
        public int percent_time_shading_verts;
        public int percent_time_compute;
        public int percent_vertex_fetch_stall;
        public int percent_texture_fetch_stall;
        public int percent_texture_l1_miss;
        public int percent_texture_l2_miss;
        public int percent_texture_nearest_filtered;
        public int percent_texture_linear_filtered;
        public int percent_texture_anisotropic_filtered;
        public int vrshell_average_frame_rate;
        public int vrshell_gpu_time_microseconds;
        public int vrshell_and_guardian_gpu_time_microseconds;
    }


    private static AndroidJavaClass _MetricsService = null;
    private static AndroidJavaObject _Context = null;

    private static bool _NativeInitialized = false;
    private static bool _IsBound = false;
    private static OVRMetricsToolSDK _Instance;

    [DllImport ("OVRMetricsTool")]
    private static extern bool ovrMetricsTool_Initialize(IntPtr jvm, IntPtr jni, IntPtr context);
    [DllImport ("OVRMetricsTool")]
    private static extern bool ovrMetricsTool_EnterVrMode();
    [DllImport ("OVRMetricsTool")]
    private static extern bool ovrMetricsTool_AppendCsvDebugString(string debugString);
    [DllImport ("OVRMetricsTool")]
    private static extern bool ovrMetricsTool_SetOverlayDebugString(string debugString);
    [DllImport ("OVRMetricsTool")]
    private static extern string ovrMetricsTool_GetLatestEventJson();
    [DllImport ("OVRMetricsTool")]
    private static extern bool ovrMetricsTool_LeaveVrMode();
    [DllImport ("OVRMetricsTool")]
    private static extern bool ovrMetricsTool_Shutdown();

    public static OVRMetricsToolSDK Instance
    {
        get
        {
            if (_Instance == null)
            {
                var go = new GameObject("OVRMetricsToolSDK") { hideFlags = HideFlags.HideAndDontSave };
                DontDestroyOnLoad(go);

                _Instance = go.AddComponent<OVRMetricsToolSDK>();
            }
            return _Instance;
        }
    }

    [System.Diagnostics.Conditional("JNI_AVAILABLE")]
    private static void Initialize()
    {
        if (_Context == null)
        {
            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            _Context = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        }

        try
        {
            _NativeInitialized = ovrMetricsTool_Initialize(IntPtr.Zero, IntPtr.Zero, _Context.GetRawObject());
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Failed to Initialize Native ovrMetricsTool plugin. Falling back to Jni.\n"+ex);
            _NativeInitialized = false;
        }

        if (!_NativeInitialized)
        {
            if (_MetricsService == null)
            {
                try
                {
                    _MetricsService = new AndroidJavaClass("com.oculus.metrics.MetricsService");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Java MetricService API is not available. Is OVRMetricsToolSDK.aar enabled?\n"+ex);
                }
            }
        }
    }

    private void Awake()
    {
        Initialize();
        EnterVrMode();
    }

    private void OnDestroy()
    {
        LeaveVrMode();
        Shutdown();
    }

    private void OnApplicationPause(bool pause)
    {
        // We need to shutdown on pause to force OVR Metrics Tool into an unbound state.
        if (pause)
        {
            LeaveVrMode();
        }
        else
        {
            EnterVrMode();
        }
    }

    private void EnterVrMode()
    {
        if (_IsBound) {
            return;
        }

        if (_NativeInitialized)
        {
            _IsBound = ovrMetricsTool_EnterVrMode();
        }
        else if (_MetricsService != null)
        {
            _MetricsService.CallStatic("bind", _Context);
            _IsBound = true;
        }
    }

    private void LeaveVrMode()
    {
        if (!_IsBound)
        {
            return;
        }

        if (_NativeInitialized)
        {
            ovrMetricsTool_LeaveVrMode();
        }
        else
        {
            _MetricsService.CallStatic("shutdown", _Context);
        }
        _IsBound = false;
    }

    private void Shutdown()
    {
        if (_NativeInitialized)
        {
            ovrMetricsTool_Shutdown();
        }

        _NativeInitialized = false;
        _MetricsService = null;
    }


    public bool AppendCsvDebugString(string debugString)
    {
        if (!_IsBound)
        {
            return false;
        }

        _IsBound = _NativeInitialized
            ? ovrMetricsTool_AppendCsvDebugString(debugString)
            : _MetricsService.CallStatic<bool>("appendCsvDebugString", _Context, debugString);

        return _IsBound;
    }

    public bool SetOverlayDebugString(string debugString)
    {
        if (!_IsBound)
        {
            return false;
        }

        _IsBound = _NativeInitialized
            ? ovrMetricsTool_SetOverlayDebugString(debugString)
            : _MetricsService.CallStatic<bool>("setOverlayDebugString", _Context, debugString);

        return _IsBound;
    }

    public MetricsSnapshot? GetLatestMetricsSnapshot()
    {
        if (!_IsBound)
        {
            return null;
        }

        string result = null;
        result = _NativeInitialized
            ? ovrMetricsTool_GetLatestEventJson()
            : _MetricsService.CallStatic<string>("getLatestEventJson", _Context);

        if (result != null)
        {
            return JsonUtility.FromJson<MetricsSnapshot>(result);
        }
        _IsBound = false;
        return null;
    }
}
