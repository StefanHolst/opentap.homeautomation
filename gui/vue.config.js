module.exports = {
    pwa: {
        name: 'OpenTAP Home Automation',
        themeColor: '#FFFFFF',
        msTileColor: '#000000',
        appleMobileWebAppCapable: 'yes',
        appleMobileWebAppStatusBarStyle: 'black',
    
        // configure the workbox plugin
        workboxPluginMode: 'InjectManifest',
        workboxOptions: {
          // swSrc is required in InjectManifest mode.
          swSrc: 'dev/sw.js',
          // ...other Workbox options...
        }
    }
}