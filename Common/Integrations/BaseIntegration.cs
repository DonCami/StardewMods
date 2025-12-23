using System;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;

namespace DonCami.Stardew.Common.Integrations;

/// <summary>The base implementation for a mod integration.</summary>
internal abstract class BaseIntegration : IModIntegration
{
    /*********
     ** Public methods
     *********/
    /// <summary>Construct an instance.</summary>
    /// <param name="label">A human-readable name for the mod.</param>
    /// <param name="modID">The mod's unique ID.</param>
    /// <param name="minVersion">The minimum version of the mod that's supported.</param>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="monitor">Encapsulates monitoring and logging.</param>
    protected BaseIntegration(string label, string modID, string minVersion, IModRegistry modRegistry, IMonitor monitor)
    {
        // init
        Label = label;
        ModID = modID;
        ModRegistry = modRegistry;
        Monitor = monitor;

        // validate mod
        var manifest = modRegistry.Get(ModID)?.Manifest;
        if (manifest == null)
            return;
        if (manifest.Version.IsOlderThan(minVersion))
        {
            monitor.Log(
                $"Detected {label} {manifest.Version}, but need {minVersion} or later. Disabled integration with this mod.",
                LogLevel.Warn);
            return;
        }

        IsLoaded = true;
    }

    /*********
     ** Fields
     *********/
    /// <summary>The mod's unique ID.</summary>
    protected string ModID { get; }

    /// <summary>An API for fetching metadata about loaded mods.</summary>
    protected IModRegistry ModRegistry { get; }

    /// <summary>Encapsulates monitoring and logging.</summary>
    protected IMonitor Monitor { get; }


    /*********
     ** Accessors
     *********/
    /// <summary>A human-readable name for the mod.</summary>
    public string Label { get; }

    /// <summary>Whether the mod is available.</summary>
    public virtual bool IsLoaded { get; }

    /// <summary>Get an API for the mod, and show a message if it can't be loaded.</summary>
    /// <typeparam name="TApi">The API type.</typeparam>
    protected TApi? GetValidatedApi<TApi>()
        where TApi : class
    {
        var api = ModRegistry.GetApi<TApi>(ModID);
        if (api == null)
        {
            Monitor.Log($"Detected {Label}, but couldn't fetch its API. Disabled integration with this mod.",
                LogLevel.Warn);
            return null;
        }

        return api;
    }

    /// <summary>Assert that the integration is loaded.</summary>
    /// <exception cref="InvalidOperationException">The integration isn't loaded.</exception>
    protected virtual void AssertLoaded()
    {
        if (!IsLoaded)
            throw new InvalidOperationException($"The {Label} integration isn't loaded.");
    }
}

/// <summary>The base implementation for a mod integration.</summary>
/// <typeparam name="TApi">The API type.</typeparam>
internal abstract class BaseIntegration<TApi> : BaseIntegration
    where TApi : class
{
    /*********
     ** Public methods
     *********/
    /// <summary>Construct an instance.</summary>
    /// <param name="label">A human-readable name for the mod.</param>
    /// <param name="modID">The mod's unique ID.</param>
    /// <param name="minVersion">The minimum version of the mod that's supported.</param>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="monitor">Encapsulates monitoring and logging.</param>
    protected BaseIntegration(string label, string modID, string minVersion, IModRegistry modRegistry, IMonitor monitor)
        : base(label, modID, minVersion, modRegistry, monitor)
    {
        if (base.IsLoaded)
            ModApi = GetValidatedApi<TApi>();
    }

    /*********
     ** Accessors
     *********/
    /// <summary>The mod's public API.</summary>
    public TApi? ModApi { get; }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(ModApi))]
    public override bool IsLoaded => ModApi != null;

    /// <inheritdoc />
    [MemberNotNull(nameof(ModApi))]
    protected override void AssertLoaded()
    {
        if (!IsLoaded)
            throw new InvalidOperationException($"The {Label} integration isn't loaded.");
    }

    /// <summary>Call an API method with error-handling.</summary>
    /// <param name="callApi">Call the API method.</param>
    /// <param name="error">
    ///     A sentence indicating what failed, including '{0}' for the other mod's name (like "Failed fetching
    ///     outputs from {0}").
    /// </param>
    protected void SafelyCallApi(Action<TApi> callApi, string error)
    {
        if (IsLoaded)
            try
            {
                callApi(ModApi);
            }
            catch (Exception ex)
            {
                error = string.Format(error, Label);

                Monitor.LogOnce($"{error}\n\nTechnical info:\n{ex}", LogLevel.Error);
            }
    }

    /// <summary>Call an API method with error-handling.</summary>
    /// <typeparam name="TReturn">The API method return value.</typeparam>
    /// <param name="callApi">Call the API method.</param>
    /// <param name="defaultValue">The default value to return if the API fails.</param>
    /// <param name="error">
    ///     A sentence indicating what failed, including '{0}' for the other mod's name (like "Failed fetching
    ///     outputs from {0}").
    /// </param>
    [return: NotNullIfNotNull(nameof(defaultValue))]
    protected TReturn? SafelyCallApi<TReturn>(Func<TApi, TReturn> callApi, string error,
        TReturn? defaultValue = default)
    {
        if (IsLoaded)
            try
            {
                return callApi(ModApi);
            }
            catch (Exception ex)
            {
                error = string.Format(error, Label);

                Monitor.LogOnce($"{error}\n\nTechnical info:\n{ex}", LogLevel.Error);
            }

        return defaultValue;
    }
}