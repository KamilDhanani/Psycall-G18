
"use client";

import React, { Suspense, useState } from "react";
import Image from "next/image";
import Link from "next/link";
import { useSearchParams } from "next/navigation";
import { Button } from "../../components/ui/button";
import { Card } from "../../components/ui/card";
import { config } from "../../config";

function ResetPasswordForm() {
  const searchParams = useSearchParams();
  const token = searchParams.get("token");

  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError("");

    if (!token) {
      setError("Invalid reset link. Please request a new one.");
      return;
    }

    if (newPassword.length < 8) {
      setError("Password must be at least 8 characters long.");
      return;
    }

    if (newPassword !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }

    setIsLoading(true);

    try {
      const response = await fetch(
        `${config.apiUrl}/api/Auth/reset-password`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            token,
            newPassword,
          }),
        }
      );

      if (!response.ok) {
        if (response.status === 400) {
          setError(
            "Invalid or expired reset link, or invalid password. Please request a new link."
          );
        } else {
          setError(
            "Unable to reset your password right now. Please try again later."
          );
        }
        return;
      }

      setIsSuccess(true);
      setNewPassword("");
      setConfirmPassword("");
    } catch (error) {
      console.error("Password reset request failed:", error);
      setError(
        "Unable to connect to the server. Please try again later."
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="flex flex-col min-h-screen">
      {/* Header */}
      <header className="bg-gray-100 dark:bg-neutral-900 text-black dark:text-white p-5 flex items-center justify-between border-b border-gray-300 dark:border-gray-700 shadow-lg">
        <div className="flex items-center">
          <Image
            src="/HCAHeader.png"
            alt="HCA Logo"
            width={50}
            height={50}
            className="mr-6"
          />
          <h1 className="text-2xl font-bold">PSYCALL</h1>
        </div>
      </header>

      {/* Reset Password Card */}
      <main className="flex-grow flex items-center justify-center bg-blue-950 dark:bg-neutral-800">
        <Card className="p-6 sm:p-10 bg-white dark:bg-neutral-900 shadow-lg rounded-3xl w-full max-w-[400px] mx-4">
          <h2 className="text-2xl font-bold mb-4 text-center text-black dark:text-white">
            Reset Password
          </h2>

          {isSuccess ? (
            <div className="space-y-4 text-center">
              <p role="status" className="text-sm text-green-600 dark:text-green-400">
                Your password has been reset successfully.
              </p>
              <p className="text-sm text-gray-700 dark:text-gray-200">
                You can now log in using your new password.
              </p>
              <Link
                href="/"
                className="block text-orange-500 hover:underline"
              >
                Return to Login
              </Link>
            </div>
          ) : !token ? (
            <div className="space-y-4 text-center">
              <p role="alert" className="text-sm text-red-600 dark:text-red-400">
                Invalid reset link. Please request a new one.
              </p>
              <Link
                href="/forgot-password"
                className="block text-orange-500 hover:underline"
              >
                Request New Reset Link
              </Link>
            </div>
          ) : (
            <form onSubmit={handleSubmit} className="space-y-4">
              <p className="text-sm text-gray-700 dark:text-gray-200 text-center">
                Enter and confirm your new password.
              </p>

              <div>
                <label
                  htmlFor="newPassword"
                  className="block text-lg font-medium text-gray-700 dark:text-gray-200"
                >
                  New Password
                </label>
                <input
                  type={showPassword ? "text" : "password"}
                  id="newPassword"
                  value={newPassword}
                  onChange={(e) => setNewPassword(e.target.value)}
                  placeholder="New Password"
                  minLength={8}
                  required
                  disabled={isLoading}
                  autoComplete="new-password"
                  className="mt-1 block w-full px-3 py-2 border border-gray-500 dark:border-gray-700 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 placeholder-gray-400 text-black dark:text-white bg-white dark:bg-neutral-800"
                />
              </div>

              <div>
                <label
                  htmlFor="confirmPassword"
                  className="block text-lg font-medium text-gray-700 dark:text-gray-200"
                >
                  Confirm Password
                </label>
                <input
                  type={showPassword ? "text" : "password"}
                  id="confirmPassword"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  placeholder="Confirm Password"
                  minLength={8}
                  required
                  disabled={isLoading}
                  autoComplete="new-password"
                  className="mt-1 block w-full px-3 py-2 border border-gray-500 dark:border-gray-700 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 placeholder-gray-400 text-black dark:text-white bg-white dark:bg-neutral-800"
                />
              </div>

              <div className="flex items-center gap-2">
                <input
                  type="checkbox"
                  id="showPassword"
                  checked={showPassword}
                  onChange={(e) => setShowPassword(e.target.checked)}
                  disabled={isLoading}
                />
                <label
                  htmlFor="showPassword"
                  className="text-sm text-gray-700 dark:text-gray-200"
                >
                  Show passwords
                </label>
              </div>

              <p className="text-xs text-gray-500 dark:text-gray-400">
                Password must contain at least 8 characters.
              </p>

              {error && (
                <p role="alert" className="text-sm text-red-600 dark:text-red-400">
                  {error}
                </p>
              )}

              <Button
                type="submit"
                className="w-full bg-orange-500 text-white py-3 rounded-md cursor-pointer hover:bg-orange-400 disabled:opacity-50"
                disabled={isLoading}
              >
                {isLoading ? "Resetting..." : "Reset Password"}
              </Button>

              <Link
                href="/"
                className="block text-center text-sm text-orange-500 hover:underline"
              >
                Back to Login
              </Link>
            </form>
          )}
        </Card>
      </main>

      {/* Footer */}
      <footer className="bg-gray-100 dark:bg-neutral-900 text-black dark:text-white p-2 text-center border-t border-gray-300 dark:border-gray-700 shadow-lg">
        <div className="flex justify-center items-center space-x-6 mb-1">
          <Image
            src="/UCF-College-of-Medicine-Footer.png"
            alt="UCF Footer Logo"
            width={150}
            height={75}
            className="object-contain"
          />
          <div className="h-16 w-px bg-gray-400 dark:bg-gray-700" />
          <Image
            src="/HCAFooter.png"
            alt="HCA Footer Logo"
            width={150}
            height={75}
            className="object-contain"
          />
        </div>
        <p className="text-sm">
          &copy; 2025 PSYCALL. All rights reserved.
        </p>
      </footer>
    </div>
  );
}

export default function ResetPassword() {
  return (
    <Suspense
      fallback={
        <div className="min-h-screen flex items-center justify-center bg-blue-950 dark:bg-neutral-800">
          <p className="text-white">Loading password reset...</p>
        </div>
      }
    >
      <ResetPasswordForm />
    </Suspense>
  );
}
