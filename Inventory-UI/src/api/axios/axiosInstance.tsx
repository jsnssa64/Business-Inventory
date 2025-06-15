import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: process.env.BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: process.env.ALLOW_CORS_CREDENTIALS === 'true', // if using cookies/session auth
});

// Optional: Add request interceptors for auth/token handling
axiosInstance.interceptors.request.use( 
    onFullfilled => {
        return onFullfilled;
    },
    onRejected => {
        console.error("Request error:", onRejected);
        return Promise.reject(onRejected);
    }
);

// Optional: Add interceptors for auth/token handling
axiosInstance.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
      console.warn("Unauthorized");
    }
    return Promise.reject(error);
  }
);

export default axiosInstance;