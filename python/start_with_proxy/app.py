from kameleo.local_api_client.kameleo_local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
from kameleo.local_api_client.models.server_py3 import Server
import time


kameleoBaseUrl = 'http://localhost:5050'
client = KameleoLocalApiClient(kameleoBaseUrl)

# Search Chrome Base Profiles
base_profiles = client.search_base_profiles(
    device_type='desktop',
    browser_product='chrome'
)

# Create a new profile with recommended settings for browser fingerprinting protection
# Choose one of the Chrome BaseProfiles
# You can set your proxy up in the set_proxy method
create_profile_request = BuilderForCreateProfile \
    .for_base_profile(base_profiles[0].id) \
    .set_recommended_defaults() \
    .set_proxy('socks5', Server(host='<proxy_host>', port=1080, id='<username>', secret='<password>')) \
    .build()
profile = client.create_profile(body=create_profile_request)

# Start the browser profile
client.start_profile(profile.id)

# Wait for 10 seconds
time.sleep(10)

# Stop the browser by stopping the Kameleo profile
client.stop_profile(profile.id)