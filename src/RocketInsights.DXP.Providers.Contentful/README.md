This project relies on configuration settings to be set. These are essentially API keys and related info needed to access the Contentful site.

While the `Environment` and `SpaceId` are not secret and can be stored elsewhere, greater care should be taken for the `API Key` as it should be kept secure.

For Development purpose, it can be simpler to just add the block to `User Secrets` (right click on project and select `Manage User Secrets`)
```JS
{
  "ContentfulOptions:DeliveryApiKey": "<API Key>",
  "ContentfulOptions:Environment": "<Environment>",
  "ContentfulOptions:SpaceId": "<space ID>"
}
```