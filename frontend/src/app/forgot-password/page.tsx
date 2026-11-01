
"use client";

import React, { useState } from "react";
import Image from "next/image";
import Link from "next/link";
import { Button } from "../../components/ui/button";
import { Card } from "../../components/ui/card";
import { config } from "../../config";

export default function ForgotPassword() {
  const [email, setEmail] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsLoading(true);
    setError("");

    try {
      const response = await fetch(
        `${config.apiUrl}/api/Auth/forgot-password`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ email: email.trim() }),
        }
      );

      if (!response.ok) {
        throw new Error("Unable to process your request. Please try again.");
      }

      setIsSubmitted(true);
    } catch (error) {
      console.error("Forgot password request failed:", error);
      setError(
        "Unable to process your request right now. Please try again later."
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

      {/* Forgot Password Card */}
      <main className="flex-grow flex items-center justify-center bg-blue-950 dark:bg-neutral-800">
        <Card className="p-6 sm:p-10 bg-white dark:bg-neutral-900 shadow-lg rounded-3xl w-full max-w-[400px] mx-4">
          <h2 className="text-2xl font-bold mb-4 text-center text-black dark:text-white">
            Forgot Password
          </h2>

          {isSubmitted ? (
            <div className="space-y-4">
              <p
                role="status"
                className="text-sm text-gray-700 dark:text-gray-200 text-center"
              >
                If an eligible account exists for that email address,
                password reset instructions have been sent.
              </p>
              <Link
                href="/"
                className="block text-center text-orange-500 hover:underline"
              >
                Return to Login
              </Link>
            </div>
          ) : (
            <form onSubmit={handleSubmit} className="space-y-4">
              <p className="text-sm text-gray-700 dark:text-gray-200 text-center">
                Enter your email address to request a password reset.
              </p>

              <div>
                <label
                  htmlFor="email"
                  className="block text-lg font-medium text-gray-700 dark:text-gray-200"
                >
                  Email
                </label>
                <input
                  type="email"
                  id="email"
                  name="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="Email"
                  className="mt-1 block w-full px-3 py-2 border border-gray-500 dark:border-gray-700 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 placeholder-gray-400 text-black dark:text-white bg-white dark:bg-neutral-800"
                  required
                  disabled={isLoading}
                />
              </div>

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
                {isLoading ? "Submitting..." : "Reset Password"}
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
        <p className="text-sm">&copy; 2025 PSYCALL. All rights reserved.</p>
      </footer>
    </div>
  );
}
