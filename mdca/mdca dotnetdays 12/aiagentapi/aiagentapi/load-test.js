import http from 'k6/http';
import { check } from 'k6';

const config = {
  url: __ENV.URL || 'https://apim-aiagent-demo.azure-api.net/agent/api/chat',
  subscriptionKey: __ENV.APIM_SUBSCRIPTION_KEY || 'your-key-here',
  payload: __ENV.RAW_PAYLOAD
    ? JSON.parse(__ENV.RAW_PAYLOAD)
    : {
        message: __ENV.MESSAGE || 'What is Azure API Management?',
      },
  targetRps: Number(__ENV.TARGET_RPS || 25),
  duration: __ENV.DURATION || '5s',
  preAllocatedVUs: Number(__ENV.PREALLOCATED_VUS || 50),
  maxVUs: Number(__ENV.MAX_VUS || 200),
};

export const options = {
  scenarios: {
    steady_rps: {
      executor: 'constant-arrival-rate',
      rate: config.targetRps,
      timeUnit: '1s',
      duration: config.duration,
      preAllocatedVUs: config.preAllocatedVUs,
      maxVUs: config.maxVUs,
    },
  },
  thresholds: {
    http_req_failed: ['rate<0.05'],
    http_req_duration: ['p(95)<3000'],
  },
};

export default function () {
  const body = JSON.stringify(config.payload);

  const res = http.post(config.url, body, {
    headers: {
      'Content-Type': 'application/json',
      'Ocp-Apim-Subscription-Key': config.subscriptionKey,
    },
  });

  if (res.status !== 200) {
    console.log(`STATUS=${res.status}`);
    console.log(`URL=${config.url}`);
    console.log(`RESP=${res.body}`);
  }

  check(res, {
    'status is 200': (r) => r.status === 200,
  });
}