﻿using Bundler.Transformers;
using Microsoft.AspNetCore.Builder;
using NUglify.Css;
using NUglify.JavaScript;
using System;

namespace Bundler
{
    /// <summary>
    /// Extension methods to register bundles and minifiers.
    /// </summary>
    public static class Extensions
    {
        // TODO: Add this to DI
        /// <summary>
        /// Gets the bundle options.
        /// </summary>
        public static Options Options { get; } = new Options();

        /// <summary>
        /// Adds Bundler to the <see cref="IApplicationBuilder"/> request execution pipeline
        /// </summary>
        /// <param name="app">The application object.</param>
        /// <param name="transformOptions">The transform options.</param>
        public static void UseBundles(this IApplicationBuilder app, Action<Options> transformOptions)
        {
            transformOptions(Options);

            foreach (ITransform transform in Options.Transforms)
            {
                app.Map(transform.Path, builder =>
                {
                    builder.UseMiddleware<BundleMiddleware>(transform);
                });
            }
        }

        /// <summary>
        /// Minifies JavaScript files (.js).
        /// </summary>
        public static void MinifyJavaScript(this IApplicationBuilder app, CodeSettings settings = null)
        {
            app.UseMiddleware<JavaScriptMiddleware>(settings ?? new CodeSettings());
        }

        /// <summary>
        /// Minifies CSS files (.css).
        /// </summary>
        public static void MinifyCss(this IApplicationBuilder app, CssSettings settings = null)
        {
            app.UseMiddleware<CssMiddleware>(settings ?? new CssSettings());
        }

        /// <summary>
        /// Adds a JavaScript bundle.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="route">The route name from where the bundle is served. Example: /my/bundle.js.</param>
        /// <param name="sourceFiles">An array of webroot relative file paths.</param>
        public static void AddJs(this Options options, string route, params string[] sourceFiles)
        {
            options.Transforms.Add(new JavaScriptMinifier(route).Include(sourceFiles));
        }

        /// <summary>
        /// Adds a JavaScript bundle.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="settings">The JavaScript minification settings.</param>
        /// <param name="route">The route name from where the bundle is served. Example: /my/bundle.js.</param>
        /// <param name="sourceFiles">An array of webroot relative file paths.</param>
        public static void AddJs(this Options options, CodeSettings settings, string route, params string[] sourceFiles)
        {
            options.Transforms.Add(new JavaScriptMinifier(route, settings).Include(sourceFiles));
        }

        /// <summary>
        /// Adds a CSS bundle.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="route">The route name from where the bundle is served. Example: /my/bundle.css.</param>
        /// <param name="sourceFiles">An array of webroot relative file paths.</param>
        public static void AddCss(this Options options, string route, params string[] sourceFiles)
        {
            options.Transforms.Add(new CssMinifier(route).Include(sourceFiles));
        }

        /// <summary>
        /// Adds a CSS bundle.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="settings">The CSS minification settings.</param>
        /// <param name="route">The route name from where the bundle is served. Example: /my/bundle.css.</param>
        /// <param name="sourceFiles">An array of webroot relative file paths.</param>
        public static void AddCss(this Options options, CssSettings settings, string route, params string[] sourceFiles)
        {
            options.Transforms.Add(new CssMinifier(route, settings).Include(sourceFiles));
        }
    }
}