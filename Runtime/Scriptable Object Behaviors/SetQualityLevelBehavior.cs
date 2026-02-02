using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "Set Quality Level Behavior_ New", menuName = "Behaviors/Application/Set Quality Level")]
public class SetQualityLevelBehavior : ScriptableObject
{
    #region User-Facing Settings Enums

    public enum QualityDefaults
    {
        Low,
        Medium,
        High,
        Max,
        Quest1,
        Quest2
    }

    /// <summary>
    /// Sets the maximum mipmap Levels for textures.
    /// </summary>
    public enum TextureQuality
    {
        /// <summary>
        /// Sets the maximum mipmap level to 2 (Quarter size).
        /// </summary>
        Low = 2,
        /// <summary>
        /// Sets the maximum mipmap level to 1 (Half size).
        /// </summary>
        Medium = 1,
        /// <summary>
        /// Sets the maximum mipmap level to 0 (Full size).
        /// </summary>
        High = 0
    }

    /// <summary>
    /// Sets the maximum LOD level/LOD bias and LOD Bias for meshes.
    /// </summary>
    public enum ModelQuality
    {
        /// <summary>
        /// Sets the maximum LOD Level to 1 and LOD Bias to 0.5.
        /// </summary>
		Low,
        /// <summary>
        /// Sets the maximum LOD Level to 0 and LOD Bias to 1.
        /// </summary>
		Medium,
        /// <summary>
        /// Sets the maximum LOD Level to 0 and LOD Bias to 2.
        /// </summary>
		High
    }

    /// <summary>
    /// Sets the number of additional lights, cookie atlas(?), vertex vs. pixel(maybe high/max is pixel?).
    /// 
    /// TODO - Document each setting
    /// </summary>
    public enum LightingQuality
    {
        Low,
        Medium,
        High
    }

    /// <summary>
    /// Determines realtime shadow quality (shadow distance, cascade levels?)
    /// 
    /// TODO - Document each setting
    /// </summary>
    public enum ShadowsQuality
    {
        Off,
        Low,
        Medium,
        High
    }

    #endregion

    private static UniversalRenderPipelineAsset currentUniversalRenderPipelineAsset => GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

    #region QualitySettings Setters 

    public void SetQualityLevel(int qualityLevel)
    {
        Debug.Log($"[SetQualityLevelBehavior] Setting quality level to {qualityLevel}");
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    public void SetLODBias(float bias = 1)
    {
        Debug.Log($"[SetQualityLevelBehavior] Setting LOD Bias to {bias}");
        QualitySettings.lodBias = bias;
    }

    public void SetMaximumLODLevel(int level)
    {
        Debug.Log($"[SetQualityLevelBehavior] Setting Maximum LOD Level to {level}");
        QualitySettings.maximumLODLevel = level;
    }

    public void SetMipMapLevel(int level)
    {
        Debug.Log($"[SetQualityLevelBehavior] Setting Mip Map Level to {level}");
        QualitySettings.globalTextureMipmapLimit = level;
    }

    public void SetAnisotropicFiltering(AnisotropicFiltering level)
    {
        Debug.Log($"[SetQualityLevelBehavior] Setting Anisotropic Filtering to {level}");
        QualitySettings.anisotropicFiltering = level;
    }

    #endregion

    #region Pipeline Setters

    public void SetAntiAliasing(MsaaQuality mode)
    {
        Debug.Log($"[SetQualityLevelBehavior] Setting MSAA to {mode}");
        currentUniversalRenderPipelineAsset.msaaSampleCount = (int)mode;
    }

    public static void SetRenderScale(float renderScale = 1)
    {
        Debug.Log($"[SetQualityLevelBehavior] Setting Render Scale to {renderScale}");
        currentUniversalRenderPipelineAsset.renderScale = renderScale;
    }

    public void SetAdditionalLightsCount(int count)
    {
        Debug.Log($"[SetQualityLevelBehavior] Setting Additional Lights Count to {count}");
        currentUniversalRenderPipelineAsset.maxAdditionalLightsCount = count;
    }

    #endregion

    #region User-Facing Setters

    public void SetQualityDefaults(QualityDefaults defaults)
	{
        Debug.Log($"[SetQualityLevelBehavior] Setting quality defaults to {defaults}");

        SetRenderScale(1);

        switch (defaults)
		{
			case QualityDefaults.Low:
				SetAntiAliasing(MsaaQuality.Disabled);
                SetTextureQuality(TextureQuality.Low);
                SetModelQuality(ModelQuality.Low);
                SetLightingQuality(LightingQuality.Low);
                SetShadowsQuality(ShadowsQuality.Low);
				break;
			case QualityDefaults.Medium:
                SetAntiAliasing(MsaaQuality._2x);
                SetTextureQuality(TextureQuality.Medium);
                SetModelQuality(ModelQuality.Medium);
                SetLightingQuality(LightingQuality.Medium);
                SetShadowsQuality(ShadowsQuality.Medium);
                break;
            case QualityDefaults.High:
                SetAntiAliasing(MsaaQuality._4x);
                SetTextureQuality(TextureQuality.High);
                SetModelQuality(ModelQuality.High);
                SetLightingQuality(LightingQuality.High);
                SetShadowsQuality(ShadowsQuality.High);
                break;
            case QualityDefaults.Max:
                SetAntiAliasing(MsaaQuality._8x);
                SetTextureQuality(TextureQuality.High);
                SetModelQuality(ModelQuality.High);
                SetLightingQuality(LightingQuality.High);
                SetShadowsQuality(ShadowsQuality.High);
                break;
			case QualityDefaults.Quest1:
                SetAntiAliasing(MsaaQuality.Disabled);
                SetTextureQuality(TextureQuality.Medium);
                SetModelQuality(ModelQuality.Low);
                SetLightingQuality(LightingQuality.Low);
                SetShadowsQuality(ShadowsQuality.Off);
                break;
            case QualityDefaults.Quest2:
                SetAntiAliasing(MsaaQuality._4x);
                SetTextureQuality(TextureQuality.Medium);
                SetModelQuality(ModelQuality.Medium);
                SetLightingQuality(LightingQuality.Medium);
                SetShadowsQuality(ShadowsQuality.Low);
                break;
        }
	}

    public void SetTextureQuality(TextureQuality quality)
    {
        Debug.Log($"[SetQualityLevelBehavior] Setting Texture Quality to {quality}");

        SetMipMapLevel((int)quality);
    }

    public void SetModelQuality(ModelQuality quality) 
    {
        Debug.Log($"[SetQualityLevelBehavior] Setting Model Quality to {quality}");

        switch (quality)
        {
            case ModelQuality.Low:
                SetMaximumLODLevel(1);
                SetLODBias(0.5f);
                break;
            case ModelQuality.Medium:
                SetMaximumLODLevel(0);
                SetLODBias(1);
                break;
            case ModelQuality.High:
                SetMaximumLODLevel(0);
                SetLODBias(2f);
                break;
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="quality"></param>
    public void SetLightingQuality(LightingQuality quality) 
    {
        Debug.Log($"[SetQualityLevelBehavior] Setting Lighting Quality to {quality}");

        switch (quality)
        {
            case LightingQuality.Low:
                SetAdditionalLightsCount(1);
                break;
            case LightingQuality.Medium:
                SetAdditionalLightsCount(4);
                break;
            case LightingQuality.High:
                SetAdditionalLightsCount(8);
                break;
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="quality"></param>
    public void SetShadowsQuality(ShadowsQuality quality) 
    {
        Debug.Log($"[SetQualityLevelBehavior] Setting Shadows Quality to {quality}");

        switch (quality)
        {
            case ShadowsQuality.Off:
                AdditionalGraphicsSettingsUtility.MainLightCastShadows = false;
                currentUniversalRenderPipelineAsset.shadowCascadeCount = 1;
                currentUniversalRenderPipelineAsset.shadowDistance = 0;
                break;
            case ShadowsQuality.Low:
                AdditionalGraphicsSettingsUtility.MainLightCastShadows = true;
                currentUniversalRenderPipelineAsset.shadowCascadeCount = 1;
                currentUniversalRenderPipelineAsset.shadowDistance = 10;
                break;
            case ShadowsQuality.Medium:
                AdditionalGraphicsSettingsUtility.MainLightCastShadows = true;
                currentUniversalRenderPipelineAsset.shadowCascadeCount = 4;
                currentUniversalRenderPipelineAsset.shadowDistance = 50;
                break;
            case ShadowsQuality.High:
                AdditionalGraphicsSettingsUtility.MainLightCastShadows = true;
                currentUniversalRenderPipelineAsset.shadowCascadeCount = 4;
                currentUniversalRenderPipelineAsset.shadowDistance = 100;
                break;
        }
    }

    #endregion

    #region UI Methods

    /// <summary>
    /// Quality level from settings is a 1 based index
    /// </summary>
    /// <param name="qualityLevel"></param>
    public void SetQualityLevelFromSettings(int qualityLevel) => SetQualityLevel(qualityLevel - 1);

    /// <summary>
	/// Anti Aliasing mode from settings is a 1 based index, correlating to the AntiAliasingMode enum
	/// </summary>
	/// <param name="mode"></param>
    public void SetAntiAliasingFromSettings(int mode)
    {
        switch (mode)
        {
            case 1:
                SetAntiAliasing(MsaaQuality._2x);
                break;
            case 2:
                SetAntiAliasing(MsaaQuality._4x);
                break;
            case 3:
                SetAntiAliasing(MsaaQuality._8x);
                break;
            default:
                SetAntiAliasing(MsaaQuality.Disabled);
                break;
        }
    }

    public void SetAnisotropicFilteringFromSettings(int mode)
    {
        switch (mode)
        {
            case 0:
            default:
                SetAnisotropicFiltering(AnisotropicFiltering.Disable);
                break;
            case 1:
                SetAnisotropicFiltering(AnisotropicFiltering.Enable);
                break;
        }
    }

    public void SetTextureQualityFromSettings(int quality)
    {
        switch (quality)
        {
            case 0:
                SetTextureQuality(TextureQuality.Low);
                break;
            case 1:
                SetTextureQuality(TextureQuality.Medium);
                break;
            case 2:
            default:
                SetTextureQuality(TextureQuality.High);
                break;
        }
    }

    public void SetModelQualityFromSettings(int quality)
    {
        switch(quality)
        {
            case 0:
                SetModelQuality(ModelQuality.Low);
                break;
            case 1:
                SetModelQuality(ModelQuality.Medium);
                break;
            case 2:
            default:
                SetModelQuality(ModelQuality.High);
                break;
        }
    }

    public void SetShadowsQualityFromSettings(int quality)
    {
        switch (quality)
        {
            case 0:
                SetShadowsQuality(ShadowsQuality.Off);
                break;
            case 1:
                SetShadowsQuality(ShadowsQuality.Low);
                break;
            case 2:
                SetShadowsQuality(ShadowsQuality.Medium);
                break;
            case 3:
            default:
                SetShadowsQuality(ShadowsQuality.High);
                break;
        }
    }

    public void SetLightingQualityFromSettings(int quality)
    {
        switch (quality)
        {
            case 0:
                SetLightingQuality(LightingQuality.Low);
                break;
            case 1:
                SetLightingQuality(LightingQuality.Medium);
                break;
            case 2:
            default:
                SetLightingQuality(LightingQuality.High);
                break;
        }
    }

    public void SetMaximumLODLevelFromSettings(float level)
    {
        SetMaximumLODLevel((int)level);
    }

    public void SetAdditionalLightsCountFromSettings(float count)
    {
        SetAdditionalLightsCount((int)count);
    }
    #endregion
}