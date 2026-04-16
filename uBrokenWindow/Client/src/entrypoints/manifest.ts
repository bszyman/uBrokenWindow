export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "uBroken Window Entrypoint",
    alias: "uBrokenWindow.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint.js"),
  },
];
