// === COMMANDS (mirror backend Auth module) ===

export interface LoginCommand {
  email: string;
  password: string;
  fingerprint?: string | null;
}

export interface RefreshTokenCommand {
  refreshToken: string;
  fingerprint?: string | null;
}

export interface LogoutCommand {
  refreshToken: string;
}

// === DTOs ===

export interface LoginCommandDto {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: string;
  userId: number;
  email: string;
  fullName: string;
  role: string;
}

/** Role names as issued by the backend. */
export const APP_ROLES = {
  administrator: 'Administrator',
  technician: 'Technician',
  reporter: 'Reporter'
} as const;
