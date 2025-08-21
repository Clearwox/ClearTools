# Changelog

All notable changes to this project will be documented in this file.

## [3.0.9] - 2025-08-21
### Fixed
- Fixed StringUtility.StripSymbols method to preserve spaces between alphanumeric characters
- Updated regex pattern from `[^a-zA-Z0-9]` to `[^a-zA-Z0-9\s]` to retain whitespace
- Enhanced test coverage with additional test cases for space preservation

### Changed
- Improved StringUtility.StripSymbols behavior for better text processing

## [3.0.8] - 2025-07-03
### Added
- Key Vault functionality: Extension on IHostBuilder for Azure Key Vault integration
- Service extension for automatically adding services by common interface

### Changed
- Refactored SmartEnum class for improved functionality and performance
- Updated version to 3.0.8

## [3.0.7] - 2025-06-30
### Added
- Comprehensive unit tests for API client and utility classes
- Enhanced API client with improved error handling and response management
- New utility classes and enhanced existing functionality
- Service extension for automatically adding services by common interface
- EditorJS models updated to latest version
- OTP utility code generation without timespan dependency

### Changed
- Refactored project structure and updated dependencies
- Enhanced API client configuration and functionality
- Improved project configuration and added new features
- Updated Azure.Storage.Blobs package to latest version
- Enhanced README documentation

### Fixed
- Various bug fixes and performance improvements

## [3.0.5] - 2025-06-24
### Changed
- Updated and added more symbols to StripSymbols method in StringUtility
- Enhanced string manipulation capabilities

## [2.1.17] - 2021-08-24
### Added
- Time ago function added to StringUtility class
- File upload to Azure with spread out functionality

### Changed
- Released NuGet package version 2.1.17

## [2.1.16] - 2022-07-21
### Added
- Azure uploads both synchronous and asynchronous methods
- HttpClientFactory implementation
- API validation middleware for request authentication
- Google reCAPTCHA resolution functionality

### Changed
- Azure blob search functionality refactored (2022-09-17)

## [2.1.2] - 2019-07-16
### Added
- PUT and DELETE request methods to IApiClient interface
- Enhanced HTTP client capabilities

### Changed
- Version updated to 2.1.2

## [2.0.x] - 2021
### Added
- Generate and validate code functionality for OTP operations
- Provision for country and currency features support
- Comprehensive file management utilities

### Changed
- Major refactoring and feature additions throughout 2021

## [1.0.0] - 2018-12-29
### Added
- Initial project setup and first commit
- Basic utility classes and functions
- Git repository initialization
- Initial README.md documentation
- Base project structure and configuration

### Features Included in Initial Release
- String manipulation utilities
- Basic cryptography functions  
- File management operations
- Image processing capabilities
- HTTP client utilities
- Number base conversion tools
