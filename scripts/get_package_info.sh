#
# INPUT VARS
#   SOURCE: ${{ github.EVENT_NAME }}
#   REF_NAME: ${{ github.REF_NAME }}
#   MAKE_RELEASE: ${{ github.event.inputs.make_release }}
#   COMMIT_SHA: ${{ github.sha }})
#

TIMESTAMP="$(date '+%Y%m%d_%H%M%S')"

PACKAGE_VERSION="none"
PRERELEASE="true"

if [ "$SOURCE" == "workflow_dispatch" ]; then
    PACKAGE_VERSION="${TIMESTAMP}-manual"
    # MAKE_RELEASE COMES FROM INPUT VARS FOR MANUAL RELEASES

elif [ "$SOURCE" == "schedule" ] ; then
    PACKAGE_VERSION="${TIMESTAMP}-nightly"
    REV_LIST="$(git rev-list )"

    # ONLY MAKE RELEASE WHEN CHANGES HAVE OCCURRED IN THE PAST 24h
    if [ -z "$(git rev-list --after='24 hours' $COMMIT_SHA)" ]; then
      MAKE_RELEASE="no"
    else
      MAKE_RELEASE="yes"
    fi

elif [ "$SOURCE" == "push" ]; then
    PACKAGE_VERSION="${REF_NAME}"

    # ONLY MAKE RELEASE IF PUSH IS FOR A VERSION TAG
    if [[ "$REF_NAME" == "v"* ]]; then
        MAKE_RELEASE="yes"
        PRERELEASE="false"
    else
        MAKE_RELEASE="no"
    fi
fi

# Redirect to null if not operating in github workflow
if [ -z "$GITHUB_OUTPUT" ]; then
    GITHUB_OUTPUT="/dev/null"
fi

PACKAGE_NAME="MB.Core.${PACKAGE_VERSION}.zip"
          
echo "source: $SOURCE"
echo "ref: $REF_NAME"
echo "version: $PACKAGE_VERSION"
echo "package: $PACKAGE_NAME"
echo "makerelease: $MAKE_RELEASE"
echo "prerelease: $PRERELEASE"

echo "package_version=$PACKAGE_VERSION" >> $GITHUB_OUTPUT
echo "package_name=$PACKAGE_NAME" >> $GITHUB_OUTPUT
echo "make_release=$MAKE_RELEASE" >> $GITHUB_OUTPUT
echo "prerelease=$PRERELEASE" >> $GITHUB_OUTPUT