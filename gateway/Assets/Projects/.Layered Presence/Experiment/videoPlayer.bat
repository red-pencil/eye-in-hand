gst-launch-1.0 -e filesrc location=video.mp4 ! qtdemux name=demux demux.video_0 ! queue ! decodebin ! videoconvert ! autovideosink demux.audio_0 ! queue! decodebin ! audioconvert ! autoaudiosink -v


gst-launch-1.0 filesrc location=video.mp4 ! qtdemux name=demux  demux.audio_0 ! queue ! decodebin ! audioconvert ! audioresample ! autoaudiosink   demux.video_0 ! queue ! decodebin ! videoconvert ! videoscale ! autovideosink