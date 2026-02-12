# FoliToolkit
自用工具集UPM包

#### 1. 确保 Unity 项目已配置 OpenUPM Scoped Registry:

Unity 的 Scoped Registries 配置保存在：
`./Packages/manifest.json`

```
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.cysharp"
      ]
    }
  ]
```

#### 2. Add package from Git URL:
```
https://github.com/Folivora0707/FoliToolkit.git?path=/Foli
```
