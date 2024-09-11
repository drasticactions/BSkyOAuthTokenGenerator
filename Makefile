ROOT=$(PWD)
PROJECT_ROOT=$(ROOT)/src/BSkyOAuthTokenGenerator
PROJECT_FILE=$(PROJECT_ROOT)/BSkyOAuthTokenGenerator.csproj
ARTIFACTS=$(ROOT)/artifacts
BUILD_DIR=$(ROOT)/build

clean:
	rm -rf $(ARTIFACTS)
	rm -rf $(BUILD_DIR)

linux: clean
	dotnet publish $(PROJECT_FILE) -c Release -o $(BUILD_DIR) -r linux-x64 --self-contained
	mkdir -p $(ARTIFACTS)
	@echo "Removing every file except the executable"
	find $(BUILD_DIR) -type f -not -name "BSkyOAuthTokenGenerator" -delete
	tar -czf $(ARTIFACTS)/BSkyOAuthTokenGenerator-linux-x64.tar.gz -C $(BUILD_DIR) .
