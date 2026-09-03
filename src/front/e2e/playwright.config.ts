import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  // Look for test files in the "tests" directory, relative to this configuration file.
  testDir: 'tests',
  /* Maximum time one test can run for. */
  timeout: 60 * 1000,
  expect: {
    // Configure expect options for all tests.
    timeout: 5000,
  },

  // Run all tests in parallel.
  fullyParallel: true,

  // Fail the build on CI if you accidentally left test.only in the source code.
  forbidOnly: !!process.env.CI,

  // Retry on CI only.
  retries: process.env.CI ? 2 : 0,

  // Opt out of parallel tests on CI.
  workers: process.env.CI ? 1 : undefined,

  // Reporter to use
  reporter:process.env.CI ? [ ['junit'], ['html', { open: 'never','outputFolder': '' }], ['list']] : 'html',

  use: {
    /* Maximum time each action such as `click()` can take. Defaults to 0 (no limit). */
    actionTimeout: 0,
    // Base URL to use in actions like `await page.goto('/')`.
    baseURL: process.env.CI ?? 'http://localhost:4031',

    // Collect trace when retrying the failed test.
    trace: 'on-first-retry',

    screenshot: 'only-on-failure',
  },
  // Configure projects for major browsers.
  projects: [
    {
      name: 'chromium',
      testDir: 'tests/desktop',
      use: { 
        ...devices['Desktop Chrome'],
        viewport: { width: 1200, height: 600 },
      },
    },
  ],
  // Run your local dev server before starting the tests.
  webServer: {
    command: 'npm run start',
    url: 'http://localhost:4031',
    reuseExistingServer: !process.env.CI,
  },
});