#!/bin/bash
set -e

APP_NAME="FFT"
BUNDLE_ID="com.mattwaltbriggs.fft"
VERSION="1.0.0"
PUBLISH_DIR="publish"
BUILD_DIR="build"

echo "=== Building universal macOS .app bundle ==="

# Clean
rm -rf "$PUBLISH_DIR" "$BUILD_DIR"
mkdir -p "$PUBLISH_DIR/arm64" "$PUBLISH_DIR/x64"

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
mkdir -p "$APP_PATH/Contents/MacOS"
mkdir -p "$APP_PATH/Contents/Resources"

# Create universal binary with lipo
echo "--- Creating universal binary ---"
lipo -create \
    "$PUBLISH_DIR/arm64/Fast_fourier_transform" \
    "$PUBLISH_DIR/x64/Fast_fourier_transform" \
    -output "$APP_PATH/Contents/MacOS/$APP_NAME"
chmod +x "$APP_PATH/Contents/MacOS/$APP_NAME"

# Copy managed assemblies (same for both architectures) from arm64 build
echo "--- Copying managed assemblies ---"
cp "$PUBLISH_DIR/arm64/"*.dll "$APP_PATH/Contents/MacOS/" 2>/dev/null || true
cp "$PUBLISH_DIR/arm64/"*.json "$APP_PATH/Contents/MacOS/" 2>/dev/null || true

# Copy native runtime libraries (arm64 and x64 in separate dirs)
mkdir -p "$APP_PATH/Contents/MacOS/runtimes"
cp -R "$PUBLISH_DIR/arm64/runtimes/osx-arm64" "$APP_PATH/Contents/MacOS/runtimes/" 2>/dev/null || true
cp -R "$PUBLISH_DIR/x64/runtimes/osx-x64" "$APP_PATH/Contents/MacOS/runtimes/" 2>/dev/null || true

# Copy any other architecture-specific native deps
for dir in "$PUBLISH_DIR/arm64/"*/; do
    dirname=$(basename "$dir")
    if [ "$dirname" != "runtimes" ] && [ -d "$dir" ]; then
        mkdir -p "$APP_PATH/Contents/MacOS/$dirname"
        cp -R "$dir"* "$APP_PATH/Contents/MacOS/$dirname/" 2>/dev/null || true
    fi
done

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
zip -r -y "../FFT_Universal_macOS.zip" "$APP_NAME.app"
cd ..

echo ""
echo "=== Done! ==="
echo "App bundle: $APP_PATH"
echo "Archive:    FFT_Universal_macOS.zip"
echo ""
echo "To run: open $APP_PATH"
echo "To distribute: share FFT_Universal_macOS.zip"
