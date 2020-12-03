# Youtube cleanup tool

# What is it
A tool to clean google play music / youtube music likes out of liked youtube playlist

# Why ???

Because I only want my liked videos in my liked videos playlist. If I wanted music there, I would have liked it on youtube myself

It's also super slow to do anything with playlists, especially when they're massive like mine

# how to run

First, you need a api key, and a oauth key.
Follow the instructions here: https://developers.google.com/identity/protocols/oauth2/openid-connect

Go to Credential manager (Control Panel -> User Accounts -> Credential Manager)

1. Create a `googleapikey` generic credential
   The 'password' field for this should be your google api key
2. Download the JSON for your oauth, and put it in the place that's read in


# General notes

1. The "playlists" returned from youtube are a mix of youtube and youtube music / imported google play music
2. Youtube somehow knows how to hide "music" ones
3. Likes aren't returned at all
4. We can get thumbnails for the playlists. Some playlists don't have thumbnails (eg, one or more videos got deleted)
5. Liked playlist for youtube is LL. Liked for youtube music is LM
6. There seems to to be no way of differentiating between music and video
7. Uploaded music migrated from google play is all private - and not accesible on youtube - even though the playlist is on youtube
8. There's an option in https://music.youtube.com - click your profile icon -> settings -> unselect "Show your liked music from YouTube". You can then use the LM playlist to act as the "delete from LL playlist" list
9. Deleting a youtube music like deletes it from your youtube likes and vice-versa. DISAPPOINTED!
10. When getting a video, the following seems to indicate if it's music
   a) contentDetails.licensedContent == true -> the ones with "Provided to YouTube by...Auto-generated by YouTube." in the description"
   b) potentially contentDetails.regionRestriction (but not sure if this can be on videos)
   c) topicDetails.topicCategories -> contains https://en.wikipedia.org/wiki/Music seems to be how they tell
   d) categoryId

listing playlists has a number of properties it takes in as per https://developers.google.com/youtube/v3/docs/channels/list
auditDetails,brandingSettings,contentDetails,contentOwnerDetails,id,localizations,snippet,statistics,status,topicDetails