import { AxiosRequestConfig } from 'axios';

const apiConfigs = {
  createGame: {
    url: '/api/game/creategame',
    method: 'post',
  },
} as { [key: string]: AxiosRequestConfig };

export default apiConfigs;
