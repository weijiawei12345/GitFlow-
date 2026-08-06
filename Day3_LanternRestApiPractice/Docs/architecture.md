# Architecture Notes

The legacy project chain is:

`UI -> APISystem.ListTemple -> encrypted POST -> static JArray_play_lantern[1] -> UI mapping`

The practice project replaces it with:

`TempleListController -> ITempleApiClient -> UnityWebRequest -> ApiResult<PagedResult<TempleDto>>`

The controller owns its `CancellationTokenSource` and request version. Starting a new
search, refresh, or page request cancels the previous request; an older completion is
also ignored when its version no longer matches. API clients do not update UI and do
not write global state.

`TempleApiSettings` selects the base URL:

- Development: `http://127.0.0.1:5057` (the local mock REST service).
- Production: the deployed REST API base URL, configured in the asset.

The original encrypted `/api/play/list_temple` endpoint is deliberately not copied into
this project. It remains part of the old application's compatibility path, while this
project practices the new REST contract in isolation.
