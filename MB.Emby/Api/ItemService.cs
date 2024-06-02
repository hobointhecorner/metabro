using EmbyClient.Dotnet.Api;
using EmbyClient.Dotnet.Client;
using EmbyClient.Dotnet.Model;

namespace MB.Emby.Api;

internal class ItemService : EmbyClient.Dotnet.Api.ItemsServiceApi
{
    public ItemService() : base() { }
    public ItemService(Configuration apiConfig) : base(apiConfig) { }

    internal static string? GetItemQueryParam(ItemQueryParam queryParam, Dictionary<ItemQueryParam, string> param)
    {
        if (param.ContainsKey(queryParam)) return param[queryParam];
        else return null;
    }

    internal static string GetItemQueryStringParam(ItemQueryParam queryParam, Dictionary<ItemQueryParam, string> param)
    {
        string? paramValue = GetItemQueryParam(queryParam, param);

        if (paramValue != null) return param[queryParam];
        else return String.Empty;
    }

    internal static bool? GetItemQueryBoolParam(ItemQueryParam queryParam, Dictionary<ItemQueryParam, string> param, bool isNullable = true)
    {
        bool parseResult;
        string? paramValue = GetItemQueryParam(queryParam, param);

        if (paramValue == null)
            if (isNullable) return null;
            else return false;
        else if (bool.TryParse(paramValue, out parseResult)) return parseResult;
        else throw new InvalidCastException($"Could not parse value '{paramValue}' as bool");
    }

    internal static int? GetItemQueryIntParam(ItemQueryParam queryParam, Dictionary<ItemQueryParam, string> param, bool isNullable = true)
    {
        int parseResult;
        string? paramValue = GetItemQueryParam(queryParam, param);

        if (paramValue == null)
            if (isNullable) return null;
            else return 0;
        else if (int.TryParse(paramValue, out parseResult)) return parseResult;
        else throw new InvalidCastException($"Could not parse value '{paramValue}' as int");
    }

    internal static double? GetItemQueryDoubleParam(ItemQueryParam queryParam, Dictionary<ItemQueryParam, string> param, bool isNullable = true)
    {
        double parseResult;
        string? paramValue = GetItemQueryParam(queryParam, param);

        if (paramValue == null)
            if (isNullable) return null;
            else return 0;
        else if (double.TryParse(paramValue, out parseResult)) return parseResult;
        else throw new InvalidCastException($"Could not parse value '{paramValue}' as double");
    }


    public QueryResultBaseItemDto GetItems(Dictionary<ItemQueryParam, string> itemQueryParam)
    {
        if (itemQueryParam.ContainsKey(ItemQueryParam.userId))
        {
            return base.GetUsersByUseridItems(
                GetItemQueryStringParam(ItemQueryParam.userId, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.artistType, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.maxOfficialRating, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasThemeSong, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasThemeVideo, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasSubtitles, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasSpecialFeature, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasTrailer, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.adjacentTo, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.minIndexNumber, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.minPlayers, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.maxPlayers, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.parentIndexNumber, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasParentalRating, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isHD, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.locationTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.excludeLocationTypes, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isMissing, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isUnaired, itemQueryParam),
                GetItemQueryDoubleParam(ItemQueryParam.minCommunityRating, itemQueryParam),
                GetItemQueryDoubleParam(ItemQueryParam.minCriticRating, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.airedDuringSeason, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.minPremiereDate, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.minDateLastSaved, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.minDateLastSavedForUser, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.maxPremiereDate, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasOverview, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasImdbId, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasTmdbId, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasTvdbId, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.excludeItemIds, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.startIndex, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.limit, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.recursive, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.searchTerm, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.sortOrder, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.parentId, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.fields, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.excludeItemTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.includeItemTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.anyProviderIdEquals, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.filters, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isFavorite, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isMovie, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isSeries, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isNews, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isKids, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isSports, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.mediaTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.imageTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.sortBy, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isPlayed, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.genres, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.officialRatings, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.tags, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.years, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.enableImages, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.enableUserData, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.imageTypeLimit, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.enableImageTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.person, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.personIds, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.personTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.studios, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.studioIds, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.artists, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.artistIds, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.albums, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.ids, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.videoTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.containers, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.audioCodecs, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.videoCodecs, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.subtitleCodecs, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.path, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.minOfficialRating, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isLocked, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isPlaceHolder, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasOfficialRating, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.groupItemsIntoCollections, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.is3D, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.seriesStatus, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.nameStartsWithOrGreater, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.artistStartsWithOrGreater, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.albumArtistStartsWithOrGreater, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.nameStartsWith, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.nameLessThan, itemQueryParam)
            );
        }
        else
        {
            return base.GetItems(
                GetItemQueryStringParam(ItemQueryParam.artistType, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.maxOfficialRating, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasThemeSong, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasThemeVideo, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasSubtitles, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasSpecialFeature, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasTrailer, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.adjacentTo, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.minIndexNumber, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.minPlayers, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.maxPlayers, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.parentIndexNumber, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasParentalRating, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isHD, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.locationTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.excludeLocationTypes, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isMissing, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isUnaired, itemQueryParam),
                GetItemQueryDoubleParam(ItemQueryParam.minCommunityRating, itemQueryParam),
                GetItemQueryDoubleParam(ItemQueryParam.minCriticRating, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.airedDuringSeason, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.minPremiereDate, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.minDateLastSaved, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.minDateLastSavedForUser, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.maxPremiereDate, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasOverview, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasImdbId, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasTmdbId, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasTvdbId, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.excludeItemIds, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.startIndex, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.limit, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.recursive, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.searchTerm, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.sortOrder, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.parentId, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.fields, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.excludeItemTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.includeItemTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.anyProviderIdEquals, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.filters, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isFavorite, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isMovie, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isSeries, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isNews, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isKids, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isSports, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.mediaTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.imageTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.sortBy, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isPlayed, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.genres, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.officialRatings, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.tags, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.years, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.enableImages, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.enableUserData, itemQueryParam),
                GetItemQueryIntParam(ItemQueryParam.imageTypeLimit, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.enableImageTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.person, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.personIds, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.personTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.studios, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.studioIds, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.artists, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.artistIds, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.albums, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.ids, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.videoTypes, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.containers, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.audioCodecs, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.videoCodecs, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.subtitleCodecs, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.path, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.userId, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.minOfficialRating, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isLocked, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.isPlaceHolder, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.hasOfficialRating, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.groupItemsIntoCollections, itemQueryParam),
                GetItemQueryBoolParam(ItemQueryParam.is3D, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.seriesStatus, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.nameStartsWithOrGreater, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.artistStartsWithOrGreater, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.albumArtistStartsWithOrGreater, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.nameStartsWith, itemQueryParam),
                GetItemQueryStringParam(ItemQueryParam.nameLessThan, itemQueryParam)
            );
        }
    }
}
