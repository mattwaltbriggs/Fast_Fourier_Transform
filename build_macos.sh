#!/bin/bash
set -e

APP_NAME="FFT"
BUNDLE_ID="com.mattwaltbriggs.fft"
VERSION="1.0.0"
PUBLISH_DIR="publish"
BUILD_DIR="build"

echo "=== Building macOS .app bundle ==="

# Clean
rm -rf "$PUBLISH_DIR" "$BUILD_DIR"

# Publish for arm64
echo "--- Publishing arm64 ---"
dotnet publish Fast_fourier_transform/Fast_fourier_transform/Fast_fourier_transform.csproj \
    -c Release \
    -r osx-arm64 \
    --self-contained true \
    -p:PublishSingleFile=false \
    -p:PublishTrimmed=false \
    -o "$PUBLISH_DIR/arm64"

# Publish for x64
echo "--- Publishing x64 ---"
dotnet publish Fast_fourier_transform/Fast_fourier_transform/Fast_fourier_transform.csproj \
    -c Release \
    -r osx-x64 \
    --self-contained true \
    -p:PublishSingleFile=false \
    -p:PublishTrimmed=false \
    -o "$PUBLISH_DIR/x64"

# Create .app bundle structure
APP_PATH="$BUILD_DIR/$APP_NAME.app"
MACOS_DIR="$APP_PATH/Contents/MacOS"
rm -rf "$APP_PATH"
mkdir -p "$MACOS_DIR"
mkdir -p "$APP_PATH/Contents/Resources"

# Copy arm64 publish output into Contents/MacOS/
echo "--- Copying arm64 runtime ---"
cp -R "$PUBLISH_DIR/arm64/"* "$MACOS_DIR/"

# Create x64 subdirectory for Intel Macs
echo "--- Copying x64 runtime ---"
mkdir -p "$MACOS_DIR/x64"
cp -R "$PUBLISH_DIR/x64/"* "$MACOS_DIR/x64/"

# Rename the arm64 host executable so the launcher can reference it
mv "$MACOS_DIR/Fast_fourier_transform" "$MACOS_DIR/Fast_fourier_transform_arm64"

# Also rename the x64 host executable
mv "$MACOS_DIR/x64/Fast_fourier_transform" "$MACOS_DIR/x64/Fast_fourier_transform_x64"

# Create launcher script
cat > "$MACOS_DIR/$APP_NAME" << 'LAUNCHER'
#!/bin/bash
DIR="$(cd "$(dirname "$0")" && pwd)"
ARCH=$(uname -m)
if [ "$ARCH" = "arm64" ]; then
    exec "$DIR/Fast_fourier_transform_arm64" "$@"
else
    exec "$DIR/x64/Fast_fourier_transform_x64" "$@"
fi
LAUNCHER
chmod +x "$MACOS_DIR/$APP_NAME"

# Create Info.plist
cat > "$APP_PATH/Contents/Info.plist" << PLIST
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>$APP_NAME</string>
    <key>CFBundleIdentifier</key>
    <string>$BUNDLE_ID</string>
    <key>CFBundleName</key>
    <string>$APP_NAME</string>
    <key>CFBundleDisplayName</key>
    <string>Fast Fourier Transform</string>
    <key>CFBundleVersion</key>
    <string>$VERSION</string>
    <key>CFBundleShortVersionString</key>
    <string>$VERSION</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleSupportedPlatforms</key>
    <array>
        <string>MacOSX</string>
    </array>
    <key>LSMinimumSystemVersion</key>
    <string>13.0</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>NSRequiresAquaSystemAppearance</key>
    <false/>
</dict>
</plist>
PLIST

echo "--- Bundle created at $APP_PATH ---"

# Create zip archive
echo "--- Creating archive ---"
cd "$BUILD_DIR"
zip -r -y -q "../FFT_Universal_macOS.zip" "$APP_NAME.app"
cd ..
ARCHIVE_SIZE=$(du -sh FFT_Universal_macOS.zip | cut -f1)

echo ""
echo "=== Done! ==="
echo "App bundle: $APP_PATH"
echo "Archive:    FFT_Universal_macOS.zip ($ARCHIVE_SIZE)"
echo ""
echo "To run: open $APP_PATH"
echo "To distribute: share FFT_Universal_macOS.zip"
